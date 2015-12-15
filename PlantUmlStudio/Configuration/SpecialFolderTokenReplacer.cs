using System;
using System.Text;
using SharpEssentials.Reflection;

namespace PlantUmlStudio.Configuration
{
    public class SpecialFolderTokenReplacer
    {
        public string Parse(string input)
        {
            var output = new StringBuilder(input.Length);
            StringBuilder currentToken = null;
            foreach (var c in input)
            {
                switch (c)
                {
                    case StartToken:
                        currentToken = new StringBuilder();
                        break;

                    case EndToken:
                        if (currentToken != null)
                        {
                            var token = currentToken.ToString();
                            var value = Enums.TryParse<Environment.SpecialFolder>(token)
                                             .Select(Environment.GetFolderPath)
                                             .GetOrElse(() => $"{StartToken}{token}{EndToken}");
                            output.Append(value);
                            currentToken = null;
                        }
                        else
                        {
                            output.Append(c);
                        }
                        break;

                    default:
                        if (currentToken == null)
                            output.Append(c);
                        else
                            currentToken.Append(c);
                        break;
                }
            }

            if (currentToken != null)
                output.Append(StartToken).Append(currentToken);    // Append any remaining characters that were part of a 'false' token.

            return output.ToString();
        }

        private const char StartToken = '<';
        private const char EndToken = '>';
    }
}