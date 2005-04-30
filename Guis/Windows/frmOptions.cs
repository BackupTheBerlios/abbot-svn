using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Abbot {
	public partial class frmOptions : Form {
		public frmOptions() {
			InitializeComponent();
		}

		private void btnOk_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void btnApply_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.Close();
		}
	}
}