using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace test
{
    public class Html
    {
        public StringBuilder Builder { get; set; }

        public Html(StringBuilder builder)
        {
            Builder = builder;
        }

        public void StartULTag()
        {
            Builder.Append("<ul>");
        }
        public void EndULTag()
        {
            Builder.Append("</ul>");
        }

        public void StartLITag()
        {
            Builder.Append("<li>");
        }
        public void EndLITag()
        {
            Builder.Append("</li>");
        }



        public void For(int from, int to,Action<int> action)
        {
            StartULTag();
            for (int i = from; i < to; i++)
            {
                StartLITag();
                {
                    action(i);
                }
                EndLITag();
            }
            EndULTag();
        }

        public void AddAttribute(string attribute, string value)
        {
            Builder.Remove(Builder.Length - 1, 1);
            Builder.Append(" ");
            Builder.Append(attribute);
            Builder.Append("='");
            Builder.Append(value);
            Builder.Append("'>");
        }
        public override string ToString()
        {
            return Builder.ToString();
        }
    }
}