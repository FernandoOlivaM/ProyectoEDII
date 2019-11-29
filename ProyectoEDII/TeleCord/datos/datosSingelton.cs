using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeleCord.datos
{
    public class datosSingelton
    {
        static datosSingelton _Instance;
        public string Nombre = string.Empty;
        public int PrivateKey = 0;
        public SecurityToken token = null;
        public static datosSingelton Datos
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new datosSingelton();
                }
                return _Instance;
            }
        }
    }
}