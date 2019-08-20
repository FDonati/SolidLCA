using AngelSix.SolidDna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace SolidLCA
{
    /// <summary>
    /// Interaction logic for CustomPropertiesUI.xaml
    /// </summary>
    public partial class CustomPropertiesUI : UserControl
    {
        #region Private Members
        private const string CustomPropertyDescription = "Description";
        private const string CustomPropertyStatus = "Status";
        private const string CustomPropertyRevision = "Revision";
        private const string CustomPropertyPartNumber = "PartNo";
        private const string CustomPropertyMaterial = "Material";
        private const string CustomPropertyManufacturingInformation = "Manufacturing Information";
        private const string CustomPropertyLength = "Length";
        private const string CustomPropertyEvaluatedLength = "Evaluated Length";
        private const string CustomPropertyFinish = "Finish";
        private const string CustomPropertySupplierName = "Supplier";
        private const string CustomPropertySupplierCode = "Supplier Number/Code";
        private const string CustomPropertyNote = "Note";

        private const string ManufacturingWeld = "WELD";
        private const string ManufacturingAssembly = "ASSEMBLY";
        private const string ManufacturingPlasma = "PLASME";
        private const string ManufacturingLaser = "LASER";
        private const string ManufacturingPurchase = "PURCHASE";
        private const string ManufacturingLathe = "LATHE";
        private const string ManufacturingDrill = "DRILL";
        private const string ManufacturingFold = "FOLD";
        private const string ManufacturingRoll = "ROLL";
        private const string ManufacturingSaw = "SAW";

        #endregion

        #region Constructor
        public CustomPropertiesUI()
        {
            InitializeComponent();
        }

        #endregion

        #region Startup

        /// <summary>
        /// Fired when the control is fully loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // By default show the No part open screen
            NoPartContent.Visibility = System.Windows.Visibility.Visible;
            MainContent.Visibility = System.Windows.Visibility.Hidden;

            // Listen out to for the active model

            Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;

        }

        /// <summary>
        /// Fired when the active SolidWorks model is change
        /// </summary>
        /// <param name="obj"></param>
        private void Application_ActiveModelInformationChanged(Model obj)
        {
            ReadDetails();
        }

        #endregion

        #region Model Events

        /// <summary>
        /// Reads all details from the active SolidWorks model
        /// </summary>
        private void ReadDetails()
        {
            ThreadHelpers.RunOnUIThread(() =>
            {
                // Get the active model
                var model = Application.ActiveModel;

                // if we have no model, or the model is not a part
                // then show the No Part screen and return
                if (model == null || (!model.IsPart && !model.IsAssembly))
                {

                    // Show no part screen
                    NoPartContent.Visibility = System.Windows.Visibility.Visible;
                    MainContent.Visibility = System.Windows.Visibility.Hidden;

                    return;
                }

                // if we got here we have a part

                // SHow the main content
                NoPartContent.Visibility = System.Windows.Visibility.Hidden;
                MainContent.Visibility = System.Windows.Visibility.Visible;

                // Get custom properties
                // Description
                DescriptionText.Text = model.GetCustomProperty(CustomPropertyDescription);

                // Status
                StatusText.Text = model.GetCustomProperty(CustomPropertyStatus, resolved: true);

                // Revision
                RevisionText.Text = model.GetCustomProperty(CustomPropertyRevision, resolved: true);

                // Part Number
                PartNumberText.Text = model.GetCustomProperty(CustomPropertyPartNumber, resolved: true);

                // Manufacturing Information

                    // Molding

                    // Folding

                    // Cutting

                    // Milling

                    // Drilling

                    // Welding

                    // Lathe

                    

                // Clear previous checks
                WeldCheckBox.IsChecked = AssemblyCheckBox.IsChecked = PlasmaCheckBox.IsChecked = PurchaseCheckBox.IsChecked
                = DrillCheckBox.IsChecked = LatheCheckBox.IsChecked = LaserCheckBox.IsChecked = FoldCheckBox.IsChecked 
                = RollCheckBox.IsChecked = SawCheckBox.IsChecked = false;

                // Read in value
                var manufacturingInfo = model.GetCustomProperty(CustomPropertyManufacturingInformation);

                // if we have some properties parse it
                if (!string.IsNullOrWhiteSpace(manufacturingInfo))
                {
                    foreach (var part in manufacturingInfo.Replace(" ", "").ToUpper().Split(','))
                    {
                        switch (part)
                        {

                            case ManufacturingWeld:
                                WeldCheckBox.IsChecked = true;
                                break;
                                    
                            case ManufacturingAssembly:
                                AssemblyCheckBox.IsChecked = true;
                                break;
                                    
                            case ManufacturingPlasma:
                                PlasmaCheckBox.IsChecked = true;
                                break;
                                    
                            case ManufacturingLaser:
                                LaserCheckBox.IsChecked = true;
                                break;
                                    
                            case ManufacturingPurchase:
                                PurchaseCheckBox.IsChecked = true;
                                break;
                                    
                            case ManufacturingLathe:
                                LatheCheckBox.IsChecked = true;
                                break;
                                    
                            case ManufacturingDrill:
                                DrillCheckBox.IsChecked = true;
                                break;
                                    
                            case ManufacturingFold:
                                FoldCheckBox.IsChecked = true;
                                break;

                            case ManufacturingRoll:
                                RollCheckBox.IsChecked = true;
                                break;

                            case ManufacturingSaw:
                                SawCheckBox.IsChecked = true;
                                break;


                        }
                    }
                }


                //############
                // Ideally rewrite as generalized function
                //############

                // Finish
                var finish = model.GetCustomProperty(CustomPropertyFinish);

                // Clear the selection first
                FinishList.SelectedIndex = -1;
                    
                // Try and find matching item
                foreach (var item in FinishList.Items)
                    if ((string)((ComboBoxItem)item).Content == finish)
                    {
                        // If so select it
                        FinishList.SelectedItem = item;
                        break;
                    }
                    
                // Supplier Name
                SupplierNameText.Text = model.GetCustomProperty(CustomPropertySupplierName);

                // Supplier Code
                SupplierCodeText.Text = model.GetCustomProperty(CustomPropertySupplierCode);

                // Note
                NoteText.Text = model.GetCustomProperty(CustomPropertyNote);

                // Mass
                MassText.Text = model.MassProperties?.MassInMetric();

                // Get all materials
                var materials = Application.GetMaterials();
                materials.Insert(0, new Material { Name = "Remove Material", Classification = "Not specified", DatabaseFileFound = false });

                MaterialList.ItemsSource = materials;
                MaterialList.DisplayMemberPath = "DisplayName";

                // Clear Selection
                MaterialList.SelectedIndex = -1;
                
                // Select existing material
                var existingMaterial = model.GetMaterial();

                // if we have a materials
                if (existingMaterial != null)
                    MaterialList.SelectedItem = materials?.FirstOrDefault(f => f.Database == existingMaterial.Database && f.Name == existingMaterial.Name);

            }
            );
                
        }
        #endregion

        #region Button Events
        /// <summary>
        /// Called when the read button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReadDetails();
        }
        
        /// <summary>
        /// Called when the reset button is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DescriptionText.Text = string.Empty;
            StatusText.Text = string.Empty;
            RevisionText.Text = string.Empty;
            PartNumberText.Text = string.Empty;
            MaterialList.SelectedIndex = -1;

            FinishList.SelectedIndex = -1;
            SupplierCodeText.Text = string.Empty;
            SupplierNameText.Text = string.Empty;

            WeldCheckBox.IsChecked = AssemblyCheckBox.IsChecked = PlasmaCheckBox.IsChecked = PurchaseCheckBox.IsChecked
            = DrillCheckBox.IsChecked = LatheCheckBox.IsChecked = LaserCheckBox.IsChecked = FoldCheckBox.IsChecked
            = RollCheckBox.IsChecked = SawCheckBox.IsChecked = false;

        }

        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = Application.ActiveModel;

            // Check if we have a part
            if (model == null || !model.IsPart)
            {
                return;
            }

            // Set custom properties

            // Description
            model.SetCustomProperty(CustomPropertyDescription, DescriptionText.Text);

            // Material
            // If user doesn't have a material selected, clear it
            if (MaterialList.SelectedIndex < 0)
            {
                model.SetMaterial(null);
            }
            // Otherwise set the material to the selected item
            else
            {
                model.SetMaterial((Material)MaterialList.SelectedItem);
            }

            // Manufacturing info
            var manufacturingInfo = new List<string>();

            if (WeldCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingWeld);

            if (AssemblyCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingAssembly);

            if (PlasmaCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingPlasma);

            if (LaserCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingLaser);

            if (PurchaseCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingPurchase);

            if (LatheCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingLathe);

            if (DrillCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingDrill);

            if (FoldCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingFold);

            if (RollCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingRoll);

            if (SawCheckBox.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingSaw);

            // Set manufacturing info
            model.SetCustomProperty(CustomPropertyManufacturingInformation, string.Join("," , manufacturingInfo));

            // Finish
            model.SetCustomProperty(CustomPropertyFinish, (string)((ComboBoxItem)FinishList.SelectedValue).Content);

            // Supplier Name
            model.SetCustomProperty(CustomPropertySupplierName, SupplierNameText.Text);

            // Supplier Code/Number
            model.SetCustomProperty(CustomPropertySupplierCode, SupplierCodeText.Text);

            // Note
            model.SetCustomProperty(CustomPropertyNote, NoteText.Text);

            // Re-Read details
            ReadDetails();


        }

        #endregion

        #region LCA functionalities

        private void SaveEcoInventFormat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Call here class to organize the saved data into Ecoinvent XML format
        }

        private void LaunchActivityBrowser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Calls SaveEcoInventoFormat_click

            // Launches activity browser

        }

        #endregion
    }
}