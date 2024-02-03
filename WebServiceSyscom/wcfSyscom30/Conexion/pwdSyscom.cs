using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace wcfSyscom30.Conexion
{
    public class pwdSyscom
    {
        private string cPassword; //string codificada
        private string cDecode; //string decodificada
        private const string cnsLetras = "uoieaUOIEA";
        private const string cnsNumeros = "0123456789";
        private const string cnsMayus = "BCDFGHJKLMNPQRSTVWXYZ";
        private const string cnsMinus = "bcdfghjklmnpqrstvwxyz";
        public pwdSyscom(string agPwd = "")
        {
            //constructor 
            cPassword = agPwd;
            cDecode = "";
        }
        ~pwdSyscom()
        {
            cPassword = "";
        }
        public void Decodificar(string agPwd = "")
        {
            string mTmpStr = "";
            string mAuxStr = "";
            int mIdx = 0;
            int mDig = 0;
            int mAscii = 0;
            cDecode = "";
            if (agPwd.Length > 0) { cPassword = agPwd; }
            if (cPassword.Length == 0) { return; }
            mIdx = 0;
            mTmpStr = "";
            foreach (char c in cPassword)
            {
                //mAscii = Encoding.GetEncoding(437).GetBytes(c.ToString())[0];
                mAscii = (int)VbAsc(c.ToString());
                if (mIdx == 0)
                {
                    mDig = (mAscii - 11) / 3;
                }
                else
                {
                    if (mAscii == 254)
                    {
                        mAscii = (int)VbAsc("ñ");
                    }
                    else if (mAscii == 255)
                    {
                        mAscii = (int)VbAsc("Ñ");
                    }
                    else { mAscii -= mDig; }
                    mTmpStr += VbChar(mAscii);
                }
                mIdx++;
            }
            mAuxStr = "";
            foreach (char c in mTmpStr)
            {
                if (char.IsDigit(c))
                {
                    mIdx = System.Convert.ToInt32(c.ToString());
                    mAuxStr += cnsLetras.Substring(mIdx, 1);
                }
                else
                {
                    switch (c)
                    {
                        case 'A':
                        case 'E':
                        case 'I':
                        case 'O':
                        case 'U':
                        case 'a':
                        case 'e':
                        case 'i':
                        case 'o':
                        case 'u':
                            mIdx = cnsLetras.IndexOf(c);
                            if (mIdx >= 0) { mAuxStr += mIdx.ToString(); }
                            break;
                        default:
                            if (char.IsLetter(c) && char.IsUpper(c))
                            {
                                mIdx = cnsMayus.IndexOf(c);
                                if (mIdx >= 0)
                                {
                                    mDig = (cnsMayus.Length - mIdx) - 1;
                                    mAuxStr += cnsMayus.Substring(mDig, 1);
                                }
                                else { mAuxStr += c; }
                            }
                            else if (char.IsLetter(c) && char.IsLower(c))
                            {
                                mIdx = cnsMinus.IndexOf(c);
                                if (mIdx >= 0)
                                {
                                    mDig = (cnsMinus.Length - mIdx) - 1;
                                    mAuxStr += cnsMinus.Substring(mDig, 1);
                                }
                                else { mAuxStr += c; }
                            }
                            else { mAuxStr += c; }
                            break;
                    }
                }
            }
            //invertir 
            for (mIdx = mAuxStr.Length - 1; mIdx > -1; mIdx--)
            {
                cDecode += mAuxStr[mIdx];
            }

        } //fin metodo
        public string contrasenna
        {
            get { return cDecode; }
            set { cPassword = value; }
        }
        private static string VbChar(int agCode)
        {
            if (agCode > 255) { throw new ArgumentOutOfRangeException("agCode", agCode, "El código ascii debe estar entre 0 and 255."); }
            return Encoding.Default.GetString(new[] { (byte)agCode });
        }
        private static short VbAsc(string agChar)
        {
            var el = Encoding.Default.GetBytes(agChar)[0];
            return Encoding.Default.GetBytes(agChar)[0];
        }
    }
}