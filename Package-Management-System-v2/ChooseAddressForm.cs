/*
 * C9519
 * Program 3
 * 15 November 2016
 * CIS 200-01
 * Form to allow users to pick an address to edit
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPVApp
{
    public partial class ChooseAddressForm : Form
    {
        private List<Address> editAddressList;  // List of addresses used to fill combo boxes

        public ChooseAddressForm(List<Address> addresses)
        {
            InitializeComponent();
            editAddressList = addresses;
        }

        internal int ChooseAddressIndex
        {
            // Precondition:  User has selected from destAddCbo
            // Postcondition: The index of the selected origin address returned
            get
            {
                return editAddressComboBox.SelectedIndex;
            }

            // Precondition:  -1 <= value < addressList.Count
            // Postcondition: The specified index is selected in destAddCbo
            set
            {
                if ((value >= -1) && (value < editAddressList.Count))
                    editAddressComboBox.SelectedIndex = value;
                else
                    throw new ArgumentOutOfRangeException("EditAddressListIndex", value,
                        "Choose a valid address to edit.");
            }
        }

        // Precondition:  User clicked on okBtn
        // Postcondition: If invalid field on dialog, keep form open and give first invalid
        //                field the focus. Else return OK and close form.
        private void okButton_Click(object sender, EventArgs e)
        {
            // Raise validating event for all enabled controls on form
            // If all pass, ValidateChildren() will be true
            if (ValidateChildren())
                this.DialogResult = DialogResult.OK;
        }

        // Precondition:  User pressed on cancelBtn
        // Postcondition: Form closes and sends Cancel result
        private void cancelButton_MouseDown(object sender, MouseEventArgs e)
        {
            // This handler uses MouseDown instead of Click event because
            // Click won't be allowed if other field's validation fails

            if (e.Button == MouseButtons.Left) // Was it a left-click?
                this.DialogResult = DialogResult.Cancel;
        }

        // Precondition:  Attempting to change focus from editAddressComboBox
        // Postcondition: If entered value is valid index, focus will change,
        //                else focus will remain and error provider message set
        private void ComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (editAddressComboBox.SelectedIndex == -1) // Didn't select anything from cbo
            {
                e.Cancel = true;
                chooseAddressErrorProvider.SetError(editAddressComboBox, "Please select an address to edit.");
            }
        }

        // Precondition:  Validating of sender not cancelled, so data OK
        //                sender is Control
        // Postcondition: Error provider cleared and focus allowed to change
        private void ComboBox_Validated(object sender, EventArgs e)
        {
            Control control = sender as Control; // Cast sender as Control
            chooseAddressErrorProvider.SetError(control, "");
        }

        // Precondition:  None
        // Postcondition: The list of addresses is used to populate the
        //                choose address combo box
        private void ChooseAddressForm_Load(object sender, EventArgs e)
        {
            foreach (Address a in editAddressList)
            {
                editAddressComboBox.Items.Add(a.Name);
            }
        }
    }
}
