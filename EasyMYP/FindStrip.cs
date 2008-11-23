#region Using directives

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace EasyMYP
{

    [DefaultEventAttribute("ItemFound")]
    public class FindStrip : ToolStrip, ISupportInitialize
    {

        private ToolStripLabel searchForToolStripLabel;
        private ToolStripTextBox searchForToolStripTextBox;
        private ToolStripButton findNowToolStripButton;
        private ToolStripComboBox searchInToolStripComboBox;
        private ToolStripLabel searchInToolStripLabel;

        private BindingSource _bindingSource;

        public FindStrip()
        {
            InitializeComponent();
        }

        // Custom tool strips need to ensure they 
        // refresh their UI from EndInit, otherwise
        // pre-configured tool strip items disappear
        // from the designer.
        #region ISupportInitialize Members
        public void BeginInit() { }
        public void EndInit()
        {
            if (this.DesignMode && this.Items.Count == 0) InitializeComponent();
        }
        #endregion

        [CategoryAttribute("Data")]
        [DescriptionAttribute("The BindingSource to search.")]
        [TypeConverterAttribute(typeof(ReferenceConverter))]
        public BindingSource BindingSource
        {
            get { return _bindingSource; }
            set { _bindingSource = value; }
        }

        void searchInToolStripComboBox_GotFocus(object sender, EventArgs e)
        {
            // Bail if no data source
            if (_bindingSource == null) return;
            if (_bindingSource.DataSource == null) return;

            this.searchInToolStripComboBox.Items.Clear();

            // Add column names to Search In list
            PropertyDescriptorCollection properties =
              ((ITypedList)_bindingSource).GetItemProperties(null);
            foreach (PropertyDescriptor property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    this.searchInToolStripComboBox.Items.Insert(0, property.Name);
                }
            }

            // Select first column name in list, if column names were added
            if (this.searchInToolStripComboBox.Items.Count > 0)
            {
                this.searchInToolStripComboBox.SelectedIndex = 0;
            }
        }

        [CategoryAttribute("Action")]
        [DescriptionAttribute("Occurs when data is found after a search.")]
        public event ItemFoundEventHandler ItemFound;
        protected virtual void OnItemFound(ItemFoundEventArgs e)
        {

            // Report find results
            if (ItemFound != null) ItemFound(this, e);
        }

        // Start find if Find Now button clicked
        private void findNowToolStripButton_Click(object sender, EventArgs e)
        {
            this.Find();
        }

        private void Find()
        {

            // Don't search if the underlying IBindingList implementation
            // doesn't support searching
            if (!((IBindingList)_bindingSource).SupportsSorting) return;

            // Don't search if nothing specified to look for
            string find = this.searchForToolStripTextBox.Text;
            if (string.IsNullOrEmpty(find)) return;

            // Don’t search of a column isn’t specified to search in
            string findIn = this.searchInToolStripComboBox.Text;
            if (string.IsNullOrEmpty(findIn)) return;

            // Get the PropertyDescriptor
            if (_bindingSource == null) return;

            PropertyDescriptorCollection properties =
              ((ITypedList)_bindingSource).GetItemProperties(null);
            PropertyDescriptor property = properties[findIn];

            // Find a value in a column
            int index = _bindingSource.Find(property, find);

            this.OnItemFound(new ItemFoundEventArgs(index));
        }

        private void InitializeComponent()
        {

            this.searchForToolStripLabel = new ToolStripLabel();
            this.searchForToolStripTextBox = new ToolStripTextBox();
            this.searchInToolStripLabel = new ToolStripLabel();
            this.searchInToolStripComboBox = new ToolStripComboBox();
            this.findNowToolStripButton = new ToolStripButton();

            this.SuspendLayout();

            // 
            // searchForToolStripLabel
            // 
            this.searchForToolStripLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.searchForToolStripLabel.Name = "searchForToolStripLabel";
            //this.searchForToolStripLabel.SettingsKey = "MainForm.searchForToolStripLabel";
            this.searchForToolStripLabel.Text = "Search for:";
            // 
            // searchForToolStripTextBox
            // 
            this.searchForToolStripTextBox.Name = "searchForToolStripTextBox";
            //this.searchForToolStripTextBox.SettingsKey = "MainForm.searchForToolStripTextBox";
            this.searchForToolStripTextBox.Size = new System.Drawing.Size(92, 25);
            // 
            // searchInToolStripLabel
            // 
            this.searchInToolStripLabel.Name = "searchInToolStripLabel";
            //this.searchInToolStripLabel.SettingsKey = "MainForm.searchInToolStripLabel";
            this.searchInToolStripLabel.Text = "Search in:";
            // 
            // searchInToolStripComboBox
            // 
            this.searchInToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.searchInToolStripComboBox.Name = "searchInToolStripComboBox";
            //this.searchInToolStripComboBox.SettingsKey = "MainForm.searchInToolStripComboBox";
            this.searchInToolStripComboBox.Size = new System.Drawing.Size(100, 25);
            this.searchInToolStripComboBox.GotFocus += new EventHandler(searchInToolStripComboBox_GotFocus);
            // 
            // findNowToolStripButton
            // 
            this.findNowToolStripButton.Name = "findNowToolStripButton";
            //this.findNowToolStripButton.SettingsKey = "MainForm.findNowToolStripButton";
            this.findNowToolStripButton.Text = "Find Next";
            this.findNowToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.findNowToolStripButton.Click += new System.EventHandler(this.findNowToolStripButton_Click);

            this.Items.AddRange(new ToolStripItem[] {
            this.searchForToolStripLabel,
            this.searchForToolStripTextBox,
            this.searchInToolStripLabel,
            this.searchInToolStripComboBox,
            this.findNowToolStripButton});

            this.ResumeLayout();
            this.PerformLayout();
        }
    }
}

