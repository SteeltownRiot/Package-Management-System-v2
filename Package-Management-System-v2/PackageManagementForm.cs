/*
 * C9519
 * Program 3
 * 15 November 2016
 * CIS 200-01
 * New features include the ability to save and retrive customer data in an external
 * file and to update stored addresses
*/

// Program 2
// CIS 200
// Fall 2016
// Due: 11/1/2016
// By: Andrew L. Wright (Students use Grading ID)

// File: Prog2Form.cs
// This class creates the main GUI for Program 2. It provides a
// File menu with About and Exit items, an Insert menu with Address and
// Letter items, and a Report menu with List Addresses and List Parcels
// items.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace UPVApp
{
    public partial class Program3Form : Form

    {
        private UserParcelView upv;                         // The UserParcelView
        private FileStream inputUPV;                        // Stream for reading upvs from a file
        private FileStream outputUPV;                       // Stream for writing upvs to a file
        BinaryFormatter formatter = new BinaryFormatter();  // Binary formmatter for serializing and dserializing upvs

        // Precondition:  None
        // Postcondition: The form's GUI is prepared for display. A few test addresses are
        //                added to the list of addresses
        public Program3Form()
        {
            InitializeComponent();

            upv = new UserParcelView();
        }

        // Precondition:  File, About menu item activated
        // Postcondition: Information about author displayed in dialog box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string NL = Environment.NewLine; // Newline shorthand

            MessageBox.Show($"Program 3{NL}By: C9519{NL}CIS 200{NL}Fall 2016",
                "About Program 3");
        }

        // Precondition:  File, Save As menu item activated
        // Postcondition: upv data is saved in user specified file 
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create and show dialog box enabling user to save file
            DialogResult result;
            string fileName; // name of file to save data

            using (SaveFileDialog fileChooser = new SaveFileDialog())
            {
                fileChooser.CheckFileExists = false; // let user create file

                // retrieve the result of the dialog box
                result = fileChooser.ShowDialog();
                fileName = fileChooser.FileName; // get specified file name
            }

            // ensure that user clicked "OK"
            if (result == DialogResult.OK)
            {
                // show error if user specified invalid file
                if (fileName == string.Empty)
                    MessageBox.Show("Invalid File Name", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    // save file via FileStream if user specified valid file
                    try
                    {
                        // open file with write access
                        FileStream outputUPV = new FileStream(fileName,
                           FileMode.Create, FileAccess.Write);

                        formatter.Serialize(outputUPV, upv);
                    }
                    // handle exception if there is a problem opening the file
                    catch (IOException)
                    {
                        // notify user if file could not be opened
                        MessageBox.Show("Error opening file", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SerializationException)
                    {
                        MessageBox.Show("Error Writing to File", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        outputUPV.Close();
                    }
                }
            }
        }

        // Precondition:  File, Open menu item activated
        // Postcondition: File chosen by user is opened and
        //                its data is stored in the upv
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create and show dialog box enabling user to open file
            DialogResult result; // result of OpenFileDialog
            string fileName; // name of file containing data

            using (OpenFileDialog fileChooser = new OpenFileDialog())
            {
                // retrieve the result of the dialog box
                result = fileChooser.ShowDialog();
                fileName = fileChooser.FileName; // Holds the selected file name
            }

            // ensure that user clicked "OK"
            if (result == DialogResult.OK)
            {
                // show error if user specified invalid file
                if (fileName == string.Empty)
                    MessageBox.Show("Enter a File Name", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    try
                    {
                        // create FileStream to obtain read access to file
                        inputUPV = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                        upv = (UserParcelView)formatter.Deserialize(inputUPV);
                    }
                    catch (IOException)
                    {
                        // notify user if file could not be opened
                        MessageBox.Show("Error opening file", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (SerializationException)
                    {
                        // notify user if no RecordSerializables in file
                        MessageBox.Show("No more records in file", string.Empty,
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        inputUPV.Close();
                    }
                }
            }
        }

        // Precondition:  File, Exit menu item activated
        // Postcondition: The application is exited
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Precondition:  Edit, Address menu item activated
        // Postcondition: Chosen address is updated according
        //                to user's input
        private void editAddressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChooseAddressForm chooseAddressForm = new ChooseAddressForm(upv.AddressList);    // The address dialog box form
            DialogResult chooseAddressResult = chooseAddressForm.ShowDialog(); // Show form as dialog and store result


            if (chooseAddressResult == DialogResult.OK) // Only select if OK
            {
                int addressAt = chooseAddressForm.ChooseAddressIndex;   // Holds index for for the addressAt variable
                Address editAddress = upv.AddressAt(addressAt);         // Holds address data
                AddressForm editAddressForm = new AddressForm();        // The address dialog box form

                editAddressForm.AddressName = editAddress.Name;
                editAddressForm.Address1 = editAddress.Address1;
                editAddressForm.Address2 = editAddress.Address2;
                editAddressForm.City = editAddress.City;
                editAddressForm.State = editAddress.State;
                editAddressForm.ZipText = editAddress.Zip.ToString("d5");

                DialogResult editAddressResult = editAddressForm.ShowDialog();  // Show form as dialog and store result

                if (editAddressResult == DialogResult.OK) // Only edit if OK
                {
                    upv.AddressAt(addressAt).Name = editAddressForm.AddressName;
                    upv.AddressAt(addressAt).Address1 = editAddressForm.Address1;
                    upv.AddressAt(addressAt).Address2 = editAddressForm.Address2;
                    upv.AddressAt(addressAt).City = editAddressForm.City;
                    upv.AddressAt(addressAt).State = editAddressForm.State;

                    try
                    {
                        upv.AddressAt(addressAt).Zip = int.Parse(editAddressForm.ZipText);
                    }
                    catch(ArgumentOutOfRangeException)
                    {
                        // notify user if zip code is invalid
                        MessageBox.Show("Enter valid zip code", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (ArgumentNullException)
                    {
                        // notify user if zip code is invalid
                        MessageBox.Show("Enter a zip code", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (OverflowException)
                    {
                        // notify user if zip code is invalid
                        MessageBox.Show("Enter valid zip code", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (FormatException)
                    {
                        // notify user if zip code is invalid
                        MessageBox.Show("Enter valid zip code", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                editAddressForm.Dispose();
            }
            chooseAddressForm.Dispose();
        }

        // Precondition:  Insert, Address menu item activated
        // Postcondition: The Address dialog box is displayed. If data entered
        //                are OK, an Address is created and added to the list
        //                of addresses
        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressForm addressForm = new AddressForm();    // The address dialog box form
            DialogResult result = addressForm.ShowDialog(); // Show form as dialog and store result

            if (result == DialogResult.OK) // Only add if OK
            {
                try
                {
                    upv.AddAddress(addressForm.AddressName, addressForm.Address1,
                        addressForm.Address2, addressForm.City, addressForm.State,
                        int.Parse(addressForm.ZipText)); // Use form's properties to create address
                }
                catch (FormatException) // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Address Validation!", "Validation Error");
                }
            }
            addressForm.Dispose(); // Best practice for dialog boxes
        }

        // Precondition:  Report, List Addresses menu item activated
        // Postcondition: The list of addresses is displayed in the addressResultsTxt
        //                text box
        private void listAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Addresses:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Address a in upv.AddressList)
            {
                result.Append(a.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
            }

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition:  Insert, Letter menu item activated
        // Postcondition: The Letter dialog box is displayed. If data entered
        //                are OK, a Letter is created and added to the list
        //                of parcels
        private void letterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LetterForm letterForm; // The letter dialog box form
            DialogResult result;   // The result of showing form as dialog

            if (upv.AddressCount < LetterForm.MIN_ADDRESSES) // Make sure we have enough addresses
            {
                MessageBox.Show("Need " + LetterForm.MIN_ADDRESSES + " addresses to create letter!",
                    "Addresses Error");
                return;
            }

            letterForm = new LetterForm(upv.AddressList); // Send list of addresses
            result = letterForm.ShowDialog();

            if (result == DialogResult.OK) // Only add if OK
            {
                try
                {
                    // For this to work, LetterForm's combo boxes need to be in same
                    // order as upv's AddressList
                    upv.AddLetter(upv.AddressAt(letterForm.OriginAddressIndex),
                        upv.AddressAt(letterForm.DestinationAddressIndex),
                        decimal.Parse(letterForm.FixedCostText)); // Letter to be inserted
                }
                catch (FormatException) // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Letter Validation!", "Validation Error");
                }
            }

            letterForm.Dispose(); // Best practice for dialog boxes
        }

        // Precondition:  Report, List Parcels menu item activated
        // Postcondition: The list of parcels is displayed in the parcelResultsTxt
        //                text box
        private void listParcelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            decimal totalCost = 0;                      // Running total of parcel shipping costs
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Parcels:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Parcel p in upv.ParcelList)
            {
                result.Append(p.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
                totalCost += p.CalcCost();
            }

            result.Append(NL);
            result.Append($"Total Cost: {totalCost:C}");

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }
    }
}