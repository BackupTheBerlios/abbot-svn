using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace Abbot {
	static class Program {

		static void Main() {
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] { new Service() };
			ServiceBase.Run(ServicesToRun);
		}
	}
}