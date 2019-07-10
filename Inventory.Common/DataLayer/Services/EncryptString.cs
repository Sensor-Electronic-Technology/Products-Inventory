using Effortless.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.DataLayer.Services {

    public class EncryptedResponce {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public string EncryptedString { get; set; }
    }

    public static class EncryptDecryptString {

        public static EncryptedResponce Encrypt(string toEncrypt) {
            EncryptedResponce responce = new EncryptedResponce();
            responce.Key= Bytes.GenerateKey();
            responce.IV = Bytes.GenerateIV();
            responce.EncryptedString=Strings.Encrypt(toEncrypt, responce.Key, responce.IV);
            return responce;
        }

        public static string Decrypt(string encrypted,byte[] key,byte[] iv) {
            return Strings.Decrypt(encrypted, key, iv);
        }

    }
}
