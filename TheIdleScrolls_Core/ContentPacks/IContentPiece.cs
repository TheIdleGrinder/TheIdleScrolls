using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.ContentPacks
{
	public interface IContentPiece
	{
		public string Id { get; }
		public string Name { get; }
		
		public bool CanActivate();
		public bool Activate();
		public bool Deactivate();
	}
}
