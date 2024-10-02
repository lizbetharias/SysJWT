using Microsoft.IdentityModel.Tokens;
using SysJWT.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SysJWT.Auth
{
    public class JwtAuthentication
    {
        private readonly string _key;

        public JwtAuthentication(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Este metodo sirve para encriptar una cadena de caracteres
        /// </summary>
        /// <param name="pUsuario"></param>
        /// <returns></returns>
        public string EncriptarMD5(string pUsuario)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(pUsuario));
                var strEncriptar = "";
                for (int i = 0; i < result.Length; i++)
                    strEncriptar += result[i].ToString("x2").ToLower();
                pUsuario = strEncriptar;
            }
            return pUsuario;
        }

        public string Authenticate(Usuario pUsuario)
        {
            // Se crea un manejador para el token JWT que permitirá generar y escribir el token.
            var tokenHandler = new JwtSecurityTokenHandler();

            // Se obtiene la clave secreta (_key), que se utilizará para firmar el token, codificada en bytes.
            var tokenKey = Encoding.ASCII.GetBytes(_key);

            // Se define el "descriptor" del token, que especifica los detalles del token (claims, expiración, firma, etc.).
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // "Subject" define los datos (claims) asociados al usuario, en este caso solo el nombre de usuario.
                Subject = new ClaimsIdentity(new Claim[]
                {
            // Se añade un claim que contiene el nombre del usuario (pUsuario.Login).
            new Claim(ClaimTypes.Name, pUsuario.Login)
                }),

                // Se especifica el tiempo de expiración del token, que será 8 horas a partir del momento de su creación.
                Expires = DateTime.UtcNow.AddHours(8),

                // Se define el algoritmo y las credenciales que se utilizarán para firmar el token.
                // Se utiliza HmacSha256 con una clave secreta (tokenKey).
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            // Se crea el token utilizando el token descriptor previamente definido.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Finalmente, se convierte el token a su representación en formato cadena (string) y se devuelve.
            return tokenHandler.WriteToken(token);
        }
    }
}

