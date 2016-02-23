using System;

namespace Barbar.TreeDistance.Util
{
    public class Scanner
    {
        private string[] m_Tokens;
        private int m_Index = 0;

        public Scanner(string line)
        {
            m_Tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool hasNextInt()
        {
            if (m_Index >= m_Tokens.Length)
            {
                return false;
            }
            for(var i = m_Index; i <= m_Tokens.Length; i++)
            {
                int value;
                if (int.TryParse(m_Tokens[i], out value))
                {
                    return true;
                }
            }
            return false;
        }

        public int nextInt()
        {
            for (var i = m_Index; i <= m_Tokens.Length; i++)
            {
                int value;
                if (int.TryParse(m_Tokens[i], out value))
                {
                    return value;
                }
            }
            throw new InvalidOperationException();
        }
    }
}
