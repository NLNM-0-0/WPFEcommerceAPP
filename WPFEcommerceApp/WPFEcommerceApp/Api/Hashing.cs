﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public class Hashing {
        public string Encrypt(string username, string password) {
            byte[] salt = Encoding.ASCII.GetBytes(username);
            var data = Encoding.UTF8.GetBytes(password);
            var hash = SaltHash(data, salt);
            return Convert.ToBase64String(hash);
        }

        byte[] GenerateSalt() {
            const int saltLength = 32;

            using(var randomNumberGenerator = new RNGCryptoServiceProvider()) {
                var randomNumber = new byte[saltLength];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
        }

        byte[] SaltHash(byte[] data, byte[] salt) {
            using(var sha256 = SHA256.Create()) {
                var combinedHash = Combine(data, salt);
                return sha256.ComputeHash(combinedHash);
            }
        }
        byte[] Combine(byte[] first, byte[] second) {
            var ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }
    }
}
