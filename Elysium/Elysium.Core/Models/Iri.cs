using System.Text.RegularExpressions;

namespace Elysium.Core.Models
{
    public partial class Iri
    {
        // path can be described as {Scheme}://{Host}/{Path} if the path is nonempty
        // and {Scheme}://{Host} if the path is empty
        public string Host { get; private init; }
        public string Scheme { get; private init; }
        public string Path { get; private init; }
        public string? Fragment { get; private init; }
        public string? Query { get; private init; }

        public Iri(string scheme, string host, string path, string? fragment, string? query)
        {
            Host = host.Trim().ToLower();
            ValidateHost(host);
            Scheme = scheme.Trim().ToLower();
            if (Scheme != "http" && Scheme != "https")
                throw new ArgumentException($"Scheme {scheme} is not supported. Must be one of http, https.");
            Path = PathSepRegex().Replace(path.Trim('/'), "/");
            Fragment = fragment;
            Query = query;
        }

        public static Iri FromUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException("The Uri must be absolute", nameof(uri));

            return new Iri(uri.Scheme, uri.Host, uri.AbsolutePath, uri.Fragment, uri.Query);
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
            return new(Scheme, Host, $"{Path}/{subpath}", Fragment, Query);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Path))
                return $"{Scheme}://{Host}";
            return $"{Scheme}://{Host}/{Path}{Query ?? string.Empty}{Fragment ?? string.Empty}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Scheme, Host, Path);
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && obj is Iri iri && iri.Host == Host && iri.Scheme == Scheme && iri.Path == Path;
        }

        public static bool operator ==(Iri left, Iri right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Iri left, Iri right)
        {
            return !(left == right);
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
        public string? Fragment { get; set; }
        public string? Query { get; set; }

        public Iri Iri => new(Scheme, Host, Path, Fragment, Query);
    }
}
