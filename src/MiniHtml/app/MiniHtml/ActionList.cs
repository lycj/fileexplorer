/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/17/2006
 * Time: 1:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace QuickZip.MiniHtml.Controls
{
	/// <summary>
	/// Description of ActionList.
	/// </summary>
	public class mhActionList : 
		System.ComponentModel.Design.DesignerActionList
	{
		private DesignerActionUIService designerActionUISvc = null;
		private IComponent control;
		
		public mhActionList( IComponent component ) : base(component) 
    	{
        this.control = component;

        // Cache a reference to DesignerActionUIService, so the
        // DesigneractionList can be refreshed.
        this.designerActionUISvc =
            GetService(typeof(DesignerActionUIService))
            as DesignerActionUIService;
    	}	
		
		public string Html
		{
			get
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("Html", false);
				return (string)myProperty.GetValue(control);
			}
			set
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("Html", false);
				myProperty.SetValue(control,value);
			}
		}
		
		public string Css
		{
			get
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("Css", false);
				return (string)myProperty.GetValue(control);
			}
			set
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("Css", false);
				myProperty.SetValue(control,value);
			}
		}
		
		public Color BackColor
		{
			get
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("BackColor", false);
				return (Color)myProperty.GetValue(control);
			}
			set
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("BackColor", false);
				myProperty.SetValue(control,value);
			}
		}
		
		public parseMode ParseMode
		{
			get
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
				System.ComponentModel.PropertyDescriptor myProperty = properties.Find("ParseMode", false);
				return (parseMode)myProperty.GetValue(control);
			}
			set
			{				
				if (MessageBox.Show("Are you sure? \r\nHtml will be cleared!","", MessageBoxButtons.OKCancel)
				    == DialogResult.OK)
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
					System.ComponentModel.PropertyDescriptor myProperty = properties.Find("ParseMode", false);
					myProperty.SetValue(control,value);
				}
			}
		}
		
		public bool BBCode
		{
			get { return ParseMode == parseMode.BBCode; }
			set { if (value) ParseMode = parseMode.BBCode; else ParseMode = parseMode.Html; }
		}
		
		public void EditHtml()
		{
			mhEditorDialog editor = new mhEditorDialog();
						
			editor.Html = Html;
			editor.Css = Css;
			if (editor.ShowDialog() == DialogResult.OK)
			{
				Html = editor.Html;
				Css = editor.Css;
			}
		}
		
		public void DefaultHtml()
		{
			Utils.AboutScreen();
		}
		
		
		
		public override DesignerActionItemCollection GetSortedActionItems()
		{
    		DesignerActionItemCollection items = new DesignerActionItemCollection();

			items.Add(new DesignerActionHeaderItem("Appearance"));
    		items.Add(new DesignerActionHeaderItem("Information"));
    		items.Add(new DesignerActionMethodItem(this, "DefaultHtml",
                     "About MiniHtml", "Information",
                     "Show about information.", true));

    		string labelCaption = "Text:";
    		switch (ParseMode) 
    		{    			
    			case parseMode.Html:
    				labelCaption = "Html:";
    				break;
    			case parseMode.BBCode:
    				labelCaption = "BBCode:";    				
    				break;    				
    		}    	
    		
    		items.Add(new DesignerActionPropertyItem("ParseMode",
                         "ParseMode:", "Appearance",
                         "Set the Parse mode."));
    		
    		items.Add(new DesignerActionPropertyItem("Html",
                	         labelCaption, "Appearance",
                    	     "Set the syntax"));    			    		    		
    			    		
    		items.Add(new DesignerActionPropertyItem("Css",
                         "Css:", "Appearance",
                         "Set the Css syntax"));
    		items.Add(new DesignerActionPropertyItem("BackColor",
                         "BackColor:", "Appearance",
                         "Set the Background color"));
    		
    		switch (ParseMode) 
    		{
    				case parseMode.Html:
    				items.Add(new DesignerActionMethodItem(this, "EditHtml",
                     	"Html Editor", "Appearance",
                     	"Sets the Html/Css syntax.", true));
    				break;
    		}
    		
    		return items;
		}
		

	}
	
	
	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")] 
	public class mhDesigner : 
         System.Windows.Forms.Design.ControlDesigner
	{
    	private DesignerActionListCollection actionLists;

    	// Use pull model to populate smart tag menu.
    	public override DesignerActionListCollection ActionLists
    	{
       	 get
        	{
            	if (null == actionLists)
            	{
                	actionLists = new DesignerActionListCollection();
                	actionLists.Add(
                    new mhActionList(this.Component));
            	}
            return actionLists;
        	}
    	}
	}
}
	
	
