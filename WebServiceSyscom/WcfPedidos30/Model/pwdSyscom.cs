using System;
using System.Text;

namespace WcfPedidos30.Model
{
    public class pwdSyscom
    {
        /// <summary>
        /// The c password
        /// </summary>
        private string cPassword; //string codificada

        /// <summary>
        /// The c decode
        /// </summary>
        private string cDecode; //string decodificada

        /// <summary>
        /// The CNS letras
        /// </summary>
        private const string cnsLetras = "uoieaUOIEA";

        /// <summary>
        /// The CNS numeros
        /// </summary>
        private const string cnsNumeros = "0123456789";

        /// <summary>
        /// The CNS mayus
        /// </summary>
        private const string cnsMayus = "BCDFGHJKLMNPQRSTVWXYZ";

        /// <summary>
        /// The CNS minus
        /// </summary>
        private const string cnsMinus = "bcdfghjklmnpqrstvwxyz";

        /// <summary>   
        /// Initializes a new instance of the <see cref="pwdSyscom"/> class.
        /// </summary>
        /// <param name="agPwd">The ag password.</param>
        public pwdSyscom(string agPwd = "")
        {
            //constructor 
            cPassword = agPwd;
            cDecode = "";
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PwdSyscom"/> class.
        /// </summary>
        ~pwdSyscom()
        {
            cPassword = "";
        }

        /// <summary>
        /// Decodificars the specified ag password.
        /// </summary>
        /// <param name="agPwd">The ag password.</param>
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

        /// <summary>
        /// Gets or sets the contrasenna.
        /// </summary>
        /// <value>
        /// The contrasenna.
        /// </value>
        public string contrasenna
        {
            get { return cDecode; }
            set { cPassword = value; }
        }

        /// <summary>
        /// Vbs the character.
        /// </summary>
        /// <param name="agCode">The ag code.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">agCode;El código ascii debe estar entre 0 and 255.</exception>
        private static string VbChar(int agCode)
        {
            if (agCode > 255) { throw new ArgumentOutOfRangeException("agCode", agCode, "El código ascii debe estar entre 0 and 255."); }
            return Encoding.Default.GetString(new[] { (byte)agCode });
        }

        /// <summary>
        /// Vbs the asc.
        /// </summary>
        /// <param name="agChar">The ag character.</param>
        /// <returns></returns>
        private static short VbAsc(string agChar)
        {
            var el = Encoding.Default.GetBytes(agChar)[0];
            return Encoding.Default.GetBytes(agChar)[0];
        }

        public void Codificar(string agPwd = "")
        {
            string mTmpStr = "";
            string mAuxStr = "";
            int mIdx = 0;
            int mDig = 0;
            int mAscii = 0;
            cPassword = "";
            if (agPwd.Length > 0) { cDecode = agPwd; }
            if (cDecode.Length == 0) { return; }
            //invertir 
            for (mIdx = cDecode.Length - 1; mIdx > -1; mIdx--)
            {
                mTmpStr += cDecode[mIdx];
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
            Random mRnd = new Random();
            mDig = mRnd.Next(35, 60);
            foreach (char c in mAuxStr)
            {
                switch (c)
                {
                    case 'ñ':
                        mAscii = 254;
                        break;
                    case 'Ñ':
                        mAscii = 255;
                        break;
                    default:
                        mAscii = (int)VbAsc(c.ToString());
                        mAscii += mDig;
                        break;
                }
                cPassword += VbChar(mAscii);
            }
            mAscii = (mDig * 3) + 11;
            cPassword = VbChar(mAscii) + cPassword;
        }

        public string Codificado
        {
            get { return cPassword; }
            set { cPassword = value; }
        }
    }
}