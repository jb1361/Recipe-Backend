using System.Globalization;
using System.IO;
using System.Text;

namespace CsgoHoldem.Api.Util
{
    public class StringWriterWithEncoding : StringWriter
    {
        public StringWriterWithEncoding( Encoding encoding ) : base(new StringBuilder(), CultureInfo.CurrentCulture)
        { 
            this.m_Encoding = encoding; 
        } 
        private readonly Encoding m_Encoding; 
        public override Encoding Encoding 
        { 
            get
            { 
                return this.m_Encoding; 
            } 
        } 
    }
}