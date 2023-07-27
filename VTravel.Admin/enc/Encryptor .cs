using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// Summary description for Encryptor
/// </summary>
public  class Encryptor
{

    private EncryptTransformer transformer;
    private byte[] initVec;
    private byte[] encKey;

    public Encryptor(EncryptionAlgorithm algId)
    {
        transformer = new EncryptTransformer(algId);
    }

    public byte[] IV
    {
        get { return initVec; }
        set { initVec = value; }
    }

    public byte[] Key
    {
        get { return encKey; }
    }


    public byte[] Encrypt(byte[] bytesData, byte[] bytesKey, byte[] initVec)
    {
        //Set up the stream that will hold the encrypted data.
        MemoryStream memStreamEncryptedData = new MemoryStream();

        transformer.IV = initVec;
        ICryptoTransform transform = transformer.GetCryptoServiceProvider(bytesKey,initVec);
        CryptoStream encStream = new CryptoStream(memStreamEncryptedData,
                                                  transform,
                                                  CryptoStreamMode.Write);
        try
        {
            //Encrypt the data, write it to the memory stream.
            encStream.Write(bytesData, 0, bytesData.Length);
        }
        catch (Exception ex)
        {
            throw new Exception("Error while writing encrypted data to the  stream: \n" + ex.Message);
        }
        //Set the IV and key for the client to retrieve
        encKey = transformer.Key;
        encStream.FlushFinalBlock();
        encStream.Close();

        //Send the data back.
        return memStreamEncryptedData.ToArray();
    }//end Encrypt

}
