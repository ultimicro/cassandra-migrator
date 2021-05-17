namespace CassandraMigrator
{
    using System;

    public readonly struct MigrationVersion : IEquatable<MigrationVersion>, IComparable<MigrationVersion>, IComparable
    {
        public const int MinMajor = 0;
        public const int MaxMajor = short.MaxValue;
        public const int MinMinor = 0;
        public const int MaxMinor = short.MaxValue;

        public MigrationVersion(int major, int minor)
        {
            if (major < MinMajor || major > MaxMajor)
            {
                throw new ArgumentOutOfRangeException(nameof(major));
            }

            if (minor < MinMinor || minor > MaxMinor)
            {
                throw new ArgumentOutOfRangeException(nameof(minor));
            }

            this.Major = major;
            this.Minor = minor;
        }

        public int Major { get; }

        public int Minor { get; }

        public static bool operator ==(MigrationVersion left, MigrationVersion right) => left.Equals(right);

        public static bool operator !=(MigrationVersion left, MigrationVersion right) => !(left == right);

        public static bool operator <(MigrationVersion left, MigrationVersion right) => left.CompareTo(right) < 0;

        public static bool operator <=(MigrationVersion left, MigrationVersion right) => left.CompareTo(right) <= 0;

        public static bool operator >(MigrationVersion left, MigrationVersion right) => left.CompareTo(right) > 0;

        public static bool operator >=(MigrationVersion left, MigrationVersion right) => left.CompareTo(right) >= 0;

        public MigrationVersion IncreaseMajor() => new(this.Major + 1, 0);

        public MigrationVersion IncreaseMinor() => new(this.Major, this.Minor + 1);

        public MigrationVersion WithMinor(int minor) => new(this.Major, minor);

        public int CompareTo(MigrationVersion other)
        {
            if (other.Major > this.Major)
            {
                return -1;
            }
            else if (other.Major < this.Major)
            {
                return 1;
            }
            else if (other.Minor > this.Minor)
            {
                return -1;
            }
            else if (other.Minor < this.Minor)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj.GetType() != this.GetType())
            {
                throw new ArgumentException($"The value is not an instance of {this.GetType()}.", nameof(obj));
            }

            return this.CompareTo((MigrationVersion)obj);
        }

        public bool Equals(MigrationVersion other) => other.Major == this.Major && other.Minor == this.Minor;

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((MigrationVersion)obj);
        }

        public override int GetHashCode() => HashCode.Combine(this.Major, this.Minor);

        public override string ToString() => $"{this.Major}.{this.Minor}";
    }
}
