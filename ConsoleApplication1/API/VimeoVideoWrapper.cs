using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.API
{
	public class VimeoVideoWrapper
	{
		public bool Error { get; set; }
		public string ErrorText { get; set; }
		public long VideoId { get; set; }
		public string VideoLink { get; set; }
		public string Thumb { get; set; }
	}
}
