using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using _Server.Interfaces;

namespace _Server.Classes {
    public class Url : IUrl {

        private string _rawUrl;
        private string _path;
        private IDictionary<string, string> _parameterDict;
        private string[] _segments;
        private string _fragment;

        public Url(string rawUrl) {
            this._rawUrl = rawUrl;
            this._parameterDict = new Dictionary<string, string>();
            this.SplitUrl();
        }

        public string RawUrl {
            get => _rawUrl;
        }

        public string Path {
            get => _path;
        }

        public IDictionary<string, string> Parameter {
            get => _parameterDict;
        }

        public int ParameterCount => _parameterDict.Count;

        public string[] Segments {
            get {
                if (_segments != null) {
                    return _segments;
                }

                return new string[] { };
            }
        }

        public string FileName {
            get {
                var fileStringRegEx = @"^\w*\.\w*$";
                var fileRegex = new Regex(fileStringRegEx);
                if (fileRegex.IsMatch(_segments.Last())) {
                    return _segments.Last();
                }
                else return "";
            }
        }

        public string Extension {
            get {
                if (!String.IsNullOrEmpty(this.FileName)) {
                    var parts = this.FileName.Split(".");
                    var sb = new StringBuilder("."); 
                    sb.Append(parts[1]); 
                    return sb.ToString();
                }
                return ""; 
            }
        }

        public string Fragment {
            get {
                if (!String.IsNullOrEmpty(_fragment))
                    return _fragment;
                return "";
            }
        }

        private void SplitUrl() {
            //split when #fragment is reached
            if (_rawUrl.Contains('#')) {
                var parts = _rawUrl.Split("#", 2, StringSplitOptions.RemoveEmptyEntries);
                _rawUrl = parts[0]; 
                _fragment = parts[1];
            }

            //get url parameters
            var extracted = _rawUrl.Split("?", 2, StringSplitOptions.RemoveEmptyEntries);
            if (!String.IsNullOrEmpty(extracted[0])) {
                var segmentsRaw = new ArrayList();
                foreach (var segment in extracted[0].Split("/", StringSplitOptions.RemoveEmptyEntries)) {
                    segmentsRaw.Add(segment);
                }
                
                var sb = new StringBuilder();
                if (segmentsRaw.Count > 0) {
                    foreach (string Segment in segmentsRaw.ToArray()) {
                        sb.Append("/");
                        sb.Append(Segment);
                    }
                }
                else sb.Append("/"); 

                _path = sb.ToString();
                _segments = sb.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);
            }

            if (extracted.Length > 1 && !String.IsNullOrEmpty(extracted[1])) {
                var keyValuePairs = extracted[1].Split("&", StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in keyValuePairs) {

                    var keyValue =
                        pair.Split("=", 2, StringSplitOptions.RemoveEmptyEntries);
                    _parameterDict.Add(keyValue[0], keyValue[1]);
                }
            }
        }
    }
}