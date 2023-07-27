using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for Encryption
/// </summary>
public class Encryption
{
    EncryptionAlgorithm algorithm = EncryptionAlgorithm.Des;

    byte[] IV = Encoding.ASCII.GetBytes("1V2BHy73");
    byte[] cipherText = null;
    byte[] key = Encoding.ASCII.GetBytes("40ZX9N46");


    public Encryption()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string Encrypt(string strplainText)
    {
        try
        { //Try to encrypt.
            //Create the encryptor.
            Encryptor enc = new Encryptor(EncryptionAlgorithm.Des);
            byte[] plainText = Encoding.ASCII.GetBytes(strplainText);

            //if ((EncryptionAlgorithm.TripleDes == algorithm) ||
            //    (EncryptionAlgorithm.Rijndael == algorithm))
            //{ //3Des only work with a 16 or 24 byte key.
            //    key = Encoding.ASCII.GetBytes("password12345678");
            //    if (EncryptionAlgorithm.Rijndael == algorithm)
            //    { // Must be 16 bytes for Rijndael.
            //        IV = Encoding.ASCII.GetBytes("init vec is big.");
            //    }
            //    else
            //    {
            //        IV = Encoding.ASCII.GetBytes("init vec");
            //    }
            //}
            //else
            //{ //Des only works with an 8 byte key. The others uses variable    length keys.
            //    //Set the key to null to have a new one generated.
            //    key = Encoding.ASCII.GetBytes("password");
            //    IV = Encoding.ASCII.GetBytes("init vec");
            //}
            // Uncomment the next lines to have the key or IV generated for    you.
            // key = null;
            // IV = null;

            enc.IV = IV;

            // Perform the encryption.
            cipherText = enc.Encrypt(plainText, key, IV);
            // Retrieve the intialization vector and key. You will need it 
            // for decryption.
            IV = enc.IV;
            key = enc.Key;

            //if (Convert.ToBase64String(cipherText).Contains("+"))
            //{
            //    //return Convert.ToBase64String(cipherText).Replace("+",";:");
            //}
          
          
            return HttpUtility.UrlEncode( Convert.ToBase64String(cipherText));
          
        }
        catch (Exception ex)
        {
            //Logging.LogException(ex, "Encrypt");
            return null;
        }

    }

    public string Decrypt(string strcipherText)
    {
        try
        { //Try to decrypt.
            //Set up your decryption, give it the algorithm and initialization  vector.
            if (strcipherText != null)
            {
                strcipherText = HttpUtility.UrlDecode(strcipherText);
                strcipherText = strcipherText.Replace(" ", "+");

                Decryptor dec = new Decryptor(algorithm);
                dec.IV = IV;
                byte[] cipherText = Convert.FromBase64String(strcipherText);

                // Go ahead and decrypt.
                byte[] plainText = dec.Decrypt(cipherText, key, IV);
                // Look at your plain text.

                return Encoding.ASCII.GetString(plainText);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            //Logging.LogException(ex, "Encrypt");
            return null;
        }


    }
}
