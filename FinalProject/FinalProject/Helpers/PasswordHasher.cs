using JetBrains.Annotations;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace FinalProject.Helpers
{
    [PublicAPI]
    public class PasswordHasher
    {
        private const int DefaultIterations = 10000;

        private class HashVersion
        {
            public short Version { get; set; }
            public int SaltSize { get; set; }
            public int HashSize { get; set; }
            public KeyDerivationPrf KeyDerivation { get; set; }
        }
        private readonly Dictionary<short, HashVersion> _versions = new Dictionary<short, HashVersion>
        {
            {
                1, new HashVersion
                {
                    Version = 1,
                    KeyDerivation = KeyDerivationPrf.HMACSHA512,
                    SaltSize = 128/8,
                    HashSize = 256/8
                }
            }
        };
        private HashVersion DefaultVersion => _versions[1];
        public bool IsLatestHashVersion(byte[] data)
        {
            var version = BitConverter.ToInt16(data, 0);
            return version == DefaultVersion.Version;
        }

        public bool IsLatestHashVersion(string data)
        {
            var dataBytes = Convert.FromBase64String(data);
            return IsLatestHashVersion(dataBytes);
        }

        public byte[] GetRandomBytes(int length)
        {
            var data = new byte[length];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(data);
            }
            return data;
        }
        public byte[] Hash(string clearText, int iterations = DefaultIterations)
        {
            var currentVersion = DefaultVersion;
            var saltBytes = GetRandomBytes(currentVersion.SaltSize);
            var versionBytes = BitConverter.GetBytes(currentVersion.Version);
            var iterationBytes = BitConverter.GetBytes(iterations);
            var hashBytes = KeyDerivation.Pbkdf2(clearText, saltBytes, currentVersion.KeyDerivation, iterations, currentVersion.HashSize);

            var indexVersion = 0;
            var indexIterations = indexVersion + 2;
            var indexSalt = indexIterations + 4;
            var indexHash = indexSalt + currentVersion.SaltSize;

            var resultBytes = new byte[2 + 4 + currentVersion.SaltSize + currentVersion.HashSize];
            Array.Copy(versionBytes, 0, resultBytes, indexVersion, 2);
            Array.Copy(iterationBytes, 0, resultBytes, indexIterations, 4);
            Array.Copy(saltBytes, 0, resultBytes, indexSalt, currentVersion.SaltSize);
            Array.Copy(hashBytes, 0, resultBytes, indexHash, currentVersion.HashSize);
            return resultBytes;
        }

        public string HashToString(string clearText, int iterations = DefaultIterations)
        {
            var data = Hash(clearText, iterations);
            return Convert.ToBase64String(data);
        }

        public bool Verify(string clearText, byte[] data)
        {
            var currentVersion = _versions[BitConverter.ToInt16(data, 0)];
            var iterations = BitConverter.ToInt32(data, 2);

            var saltBytes = new byte[currentVersion.SaltSize];
            var hashBytes = new byte[currentVersion.HashSize];

            var indexSalt = 2 + 4;
            var indexHash = indexSalt + currentVersion.SaltSize;

            Array.Copy(data, indexSalt, saltBytes, 0, currentVersion.SaltSize);
            Array.Copy(data, indexHash, hashBytes, 0, currentVersion.HashSize);

            var verificationHashBytes = KeyDerivation.Pbkdf2(clearText, saltBytes, currentVersion.KeyDerivation, iterations, currentVersion.HashSize);
            return hashBytes.SequenceEqual(verificationHashBytes);
        }

        public bool Verify(string clearText, string data)
        {
            var dataBytes = Convert.FromBase64String(data);
            return Verify(clearText, dataBytes);
        }
    }
}
