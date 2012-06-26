using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PlantUmlEditor.Model
{
    public class DiagramFile
    {
        public string DiagramFilePath { get; set; }
        public string ImageFilePath { get; set; }
        public string Content { get; set; }

        public string Preview
        {
            get
            {
                // Ignore first @startuml line and select non-empty lines
                return Content.Length > 100 ? Content.Substring(0, 100) : Content;
            }
        }

        public string DiagramFileNameOnly
        {
            get
            {
                return Path.GetFileName(this.DiagramFilePath);
            }
        }

        public string ImageFileNameOnly
        {
            get
            {
                return Path.GetFileName(this.ImageFilePath);
            }
        }

		/// <see cref="object.Equals(object)"/>
        public override bool Equals(object obj)
        {
            var other = obj as DiagramFile;
			if (other == null)
				return false;

            return other.DiagramFilePath == DiagramFilePath;
        }

		/// <see cref="object.GetHashCode"/>
        public override int GetHashCode()
        {
            return DiagramFilePath.GetHashCode();
        }
    }
}
