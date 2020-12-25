
using System.Security.Cryptography;
using System.Text;


namespace Estoque.Helpers
{
    public static class CriptoHelper
    {
        public static string HashMD5(string val)
        {
            //pegamos os bytes, ai convertemos de string para bytes
            var bytes = Encoding.ASCII.GetBytes(val);
            //aqui ele vai Criar o hash vai criptografar.
            var md5 = MD5.Create();
            //neste momento vai obter a criptografia apartir dos bytes
            var hash = md5.ComputeHash(bytes);

            var ret = string.Empty;
            for (int i = 0; i < hash.Length; i++)
            {
                //no meu for eu percoro para convertendo hash para string 
                ret += hash[i].ToString("x2");
            }
            //Aqui eu retorno o hash convertido em string
            return ret;
        }
    }
}