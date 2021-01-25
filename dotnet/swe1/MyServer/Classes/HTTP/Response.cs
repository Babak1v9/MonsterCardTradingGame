using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MTCGserver.Interfaces;

namespace MTCGserver.Classes {
    public class Response : IResponse {
        private readonly IDictionary<string, string> _headers;
        private int _statusCode;
        private string _status;
        private string _content;

        public Response() {
            _headers = new Dictionary<string, string>();
            _headers.Add("Server", "BIF-SWE1-Server");
            _headers.Add("Content-Length", "0");
        }

        public IDictionary<string, string> Headers => _headers;

        public int ContentLength {
            get => Int32.Parse(_headers["Content-Length"]);
        }

        public string ContentType {
            get => _headers.ContainsKey("Content-Type") ? _headers["Content-Type"] : "";
            set {
                if (!_headers.ContainsKey("Content-Type")) _headers.Add("Content-Type", value);
                else _headers["Content-Type"] = value;
            }
        }

        public int StatusCode {
            get {
                if (_statusCode == 0) {
                    throw new Exception("Status code not set");
                }

                return _statusCode;
            }
            set {
                if (1 <= value && value <= 511) {
                    _statusCode = value;
                    SetStatusFromCode();
                }
                else {
                    throw new Exception($"Invalid Status provided: '{value}'");
                }
            }
        }

        public string Status {
            get {
                if (String.IsNullOrEmpty(_status)) {
                    throw new Exception("No status code set");
                }

                return _status;
            }
        }

        public void AddHeader(string header, string value) {
            if (_headers.ContainsKey(header)) _headers[header] = value;
            else _headers.Add(header, value);
        }

        public string ServerHeader {
            get => _headers["Server"];
            set {
                if (!String.IsNullOrEmpty(value)) {
                    _headers["Server"] = value;
                }
                else {
                    throw new Exception("No value for Server header");
                }
            }
        }

        public void InvalidURL() {
            StatusCode = 404;
            SetContent("404. That's an error. The requested URL was not found on this server.");
        }
        public void UnauthenticatedUser() {
            StatusCode = 401;
            SetContent("Unauthorized.");
        }

        public void SendException(Exception e) {
            StatusCode = 400;
            SetContent("Exception trigger: " + Environment.NewLine + e.Message + Environment.NewLine + e.Source + Environment.NewLine + e.StackTrace);
        }

        public void SetContent(string content) {
            _content = Environment.NewLine + content + Environment.NewLine;
            _headers["Content-Length"] = $"{_content.Length}";
        }

        public void SetContent(byte[] content) {
            _content = Encoding.UTF8.GetString(content);
            _headers["Content-Length"] = $"{_content.Length}";
        }

        public void SetContent(Stream stream) {
            var reader = new StreamReader(stream);
            _content = Environment.NewLine + reader.ReadToEnd() + Environment.NewLine;
            _headers["Content-Length"] = $"{_content.Length}";
        }

        public void Send(Stream network) {
            var writer = new StreamWriter(network);
            var builder = new StringBuilder();

            builder.Append($"HTTP/1.1 {_status}{Environment.NewLine}");
            foreach (var header in _headers) {
                builder.Append($"{header.Key}: {header.Value}{Environment.NewLine}");
            }

            builder.Append(Environment.NewLine); 
            if (!String.IsNullOrEmpty(_content)) {
                builder.Append(_content);
            }

            writer.Write(builder.ToString());
            writer.Flush();
        }

        private void SetStatusFromCode() {
            _status = _statusCode switch
            {
                200 => "200 OK",
                202 => "202 Accepted",
                400 => "400 Bad Request",
                401 => "401 Unauthorized",
                403 => "403 Forbidden",
                404 => "404 Not Found",
                405 => "405 Method Not Allowed",
                406 => "406 Not Acceptable",
                408 => "408 Request Timeout",
                411 => "411 Length Required",
                500 => "500 Internal Server Error",
                501 => "501 Not Implemented",
                _ => throw new Exception("Invalid Http Status Code"),
            };
        }
    }
}