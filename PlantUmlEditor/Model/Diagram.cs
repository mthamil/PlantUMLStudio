using System.IO;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Represents a diagram.
	/// </summary>
    public class Diagram
    {
		/// <summary>
		/// The diagram's full file path.
		/// </summary>
        public string DiagramFilePath { get; set; }

		/// <summary>
		/// Just the diagram's file name.
		/// </summary>
		public string DiagramFileNameOnly
		{
			get { return Path.GetFileName(DiagramFilePath); }
		}

		/// <summary>
		/// Tje file path of the diagram's compiled image output.
		/// </summary>
        public string ImageFilePath { get; set; }

		/// <summary>
		/// Just the diagram image's file name.
		/// </summary>
		public string ImageFileNameOnly
		{
			get { return Path.GetFileName(ImageFilePath); }
		}

		/// <summary>
		/// The diagram's content.
		/// </summary>
        public string Content { get; set; }

		/// <summary>
		/// A preview of part of a diagram's content.
		/// </summary>
        public string Preview
        {
            get
            {
                // Ignore first @startuml line and select non-empty lines
                return Content.Length > 100 ? Content.Substring(0, 100) : Content;
            }
        }

		/// <see cref="object.Equals(object)"/>
        public override bool Equals(object obj)
        {
            var other = obj as Diagram;
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
