﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elysium.Hosting.Models
{
    public partial class Iri
    {
        // path can be described as {Scheme}://{Host}/{Path} if the path is nonempty
        // and {Scheme}://{Host} if the path is empty
        public string Host { get; private init; }
        public string Scheme { get; private init; }
        public string Path { get; private init; }

        public Iri(string scheme, string host, string path)
        {
            Host = host.Trim().ToLower();
            ValidateHost(host);
            Scheme = scheme.Trim().ToLower();
            if (Scheme != "http" && Scheme != "https")
                throw new ArgumentException($"Scheme {scheme} is not supported. Must be one of http, https.");
            Path = PathSepRegex().Replace(path.TrimStart('/'), "/");
        }

        public static Iri FromUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException("The Uri must be absolute", nameof(uri));

            return new Iri(uri.Scheme, uri.Host, uri.AbsolutePath);
        }

        public static Iri FromUrlEncodedString(string urlEncodedString)
        {
            if (string.IsNullOrWhiteSpace(urlEncodedString))
                throw new ArgumentException("URL encoded string cannot be null or empty", nameof(urlEncodedString));

            string decodedString = Uri.UnescapeDataString(urlEncodedString);

            if (!Uri.TryCreate(decodedString, UriKind.Absolute, out var uri))
                throw new ArgumentException("The provided URL encoded string is not a valid absolute URI", nameof(urlEncodedString));

            return FromUri(uri);
        }

        public static Iri FromUnencodedString(string unencodedString)
        {
            if (string.IsNullOrWhiteSpace(unencodedString))
                throw new ArgumentException("Unencoded string cannot be null or empty", nameof(unencodedString));

            if (!Uri.TryCreate(unencodedString, UriKind.Absolute, out var uri))
                throw new ArgumentException("The provided unencoded string is not a valid absolute URI", nameof(unencodedString));

            return FromUri(uri);
        }

        private void ValidateHost(string host)
        {
            if (host.Length > 255)
                throw new ArgumentException("Host cannot exceed 255 characters");

            var labels = host.Split('.');

            foreach (var label in labels)
            {
                if (label.Length < 1 || label.Length > 63)
                    throw new ArgumentException("Each label in the host must be between 1 and 63 characters long");

                if (!HostLabelRegex().IsMatch(label) || label.StartsWith("-") || label.EndsWith("-"))
                    throw new ArgumentException("Host labels must only contain alphanumeric characters or hyphens and cannot start or end with a hyphen.");
            }
        }

        public static implicit operator Uri(Iri iri)
        {
            return new Uri(iri.ToString());
        }

        public Iri Concatenate(string subpath)
        {
            subpath = PathSepRegex().Replace(subpath.TrimStart('/').TrimEnd('/'), "/");
            if (subpath.Length == 0)
                return this;
            return new(Scheme, Host, $"{Path}/{subpath}");
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Path))
                return $"{Scheme}://{Host}";
            return $"{Scheme}://{Host}/{Path}";
        }

        [GeneratedRegex(@"/{2,}")]
        private static partial Regex PathSepRegex();
        [GeneratedRegex(@"^[a-zA-Z0-9-_]+$")]
        private static partial Regex HostLabelRegex();
    }

    public class IriBuilder
    {
        public required string Host { get; set; }
        public string Scheme { get; set; } = "https";
        public string Path { get; set; } = "";

        public Iri Iri => new(Scheme, Host, Path);
    }
}
