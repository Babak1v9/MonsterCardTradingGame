using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MTCGserver.Interfaces;

namespace MTCGserver.Classes.HTTP {
    public class Request : IRequest {
        private readonly string _rawText;
        private string _method;
        private IUrl _url;
        private string _protocol;
        private IDictionary<string, string> _headers;
        private Stream _contentStream;
        private string _contentString;
        private byte[] _contentBytes;

        public Request(string rawRequest) {
            _rawText = rawRequest;
            SplitRawRequest();
        }

        public bool IsValid {
            get {
                if (_method.Length != 0 && _url != null) {
                    return true;
                }
                return false;
            }
        }

        public string Method => _method;

        public IUrl Url => _url;

        public IDictionary<string, string> Headers => _headers;

        public string UserAgent {
            get {
                if (_headers.ContainsKey("user-agent")) {
                    return _headers["user-agent"];
                }
                return "";
            }
        }

        public int HeaderCount => Headers.Count;

        public int ContentLength {
            get {
                if (_headers.ContainsKey("content-length")) {
                    return Int32.Parse(_headers["content-length"]);
                }

                return 0;
            }
        }

        public string ContentType {
            get {
                if (_headers.ContainsKey("content-type"))
                    return _headers["content-type"];

                return "";
            }
        }

        public Stream ContentStream => _contentStream;

        public string ContentString => _contentString;

        public byte[] ContentBytes => _contentBytes;

        private void SplitRawRequest() {
            var requestLines = _rawText.Split(Environment.NewLine);
            var bodyStartIndex = Array.IndexOf(requestLines, String.Empty);
            var requestStart = requestLines[0];
            var headerLines = new string[bodyStartIndex - 1];
            var bodyLines = new string[requestLines.Length - bodyStartIndex - 1];

            Array.Copy(requestLines, 1, headerLines, 0, bodyStartIndex - 1);
            Array.Copy(requestLines, bodyStartIndex + 1, bodyLines, 0, requestLines.Length - bodyStartIndex - 1);

            var methodAndUrl = requestStart.Split(" ", 3);
            if (methodAndUrl.Length < 3) throw new InvalidDataException("Method/Url length invalid!");

            string[] validMethods = { "GET", "POST", "PUT", "PATCH", "DELETE" };

            _method = validMethods.Contains(methodAndUrl[0]) ? methodAndUrl[0] : "";
            _url = new Url(methodAndUrl[1]);
            _protocol = methodAndUrl[2];
            _headers = ExtractHeaders(headerLines);

            //if body empty
            if (!bodyLines[0].Equals(String.Empty)) {
                _contentString = String.Join(Environment.NewLine, bodyLines);

                _contentBytes = Encoding.UTF8.GetBytes(_contentString);

                _contentStream = new MemoryStream(_contentBytes);
            }
        }

        private IDictionary<string, string> ExtractHeaders(string[] headers) {
            IDictionary<string, string> headerDict = new Dictionary<string, string>();

            foreach (var header in headers) {
                var keyValue = header.Split(": ");
                headerDict.Add(keyValue[0].ToLower(), keyValue[1]);
            }
            return headerDict;
        }
    }
}