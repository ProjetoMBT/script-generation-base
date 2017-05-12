import oracle.oats.scripting.modules.basic.api.*;
import oracle.oats.scripting.modules.browser.api.*;
import oracle.oats.scripting.modules.functionalTest.api.*;
import oracle.oats.scripting.modules.utilities.api.*;
import oracle.oats.scripting.modules.utilities.api.sql.*;
import oracle.oats.scripting.modules.utilities.api.xml.*;
import oracle.oats.scripting.modules.utilities.api.file.*;
import oracle.oats.scripting.modules.webdom.api.*;

public class script extends IteratingVUserScript {
	@ScriptService oracle.oats.scripting.modules.utilities.api.UtilitiesService utilities;
	@ScriptService oracle.oats.scripting.modules.browser.api.BrowserService browser;
	@ScriptService oracle.oats.scripting.modules.functionalTest.api.FunctionalTestService ft;
	@ScriptService oracle.oats.scripting.modules.webdom.api.WebDomService web;
	
	public void initialize() throws Exception {
		browser.launch();
	}
		
	/**
	 * Add code to be executed each iteration for this virtual user.
	 */
	public void run() throws Exception {
		beginStep("[1] EPESI (/epesi/)", 0);
		{
			web.window(2, "{{obj.Epesi_CadUsuarioL.web_window_0}}").waitForPage(null);
			{
				think(1.455);
			}
			web.textBox(4, "{{obj.Epesi_CadUsuarioL.web_input_text_username}}").click();
			{
				think(0.599);
			}
			web.textBox(5, "{{obj.Epesi_CadUsuarioL.web_input_text_username}}").setText("admin");
			{
				think(0.253);
			}
			web.textBox(6, "{{obj.Epesi_CadUsuarioL.web_input_text_username}}").pressTab();
			{
				think(1.275);
			}
			web.textBox(7,"{{obj.Epesi_CadUsuarioL.web_input_password_password}}").setPassword(deobfuscate("CzMwzF+2+fwXkAwc6LSY/g=="));
			{
				think(0.315);
			}
			web.textBox(8,"{{obj.Epesi_CadUsuarioL.web_input_password_password}}").pressEnter();
		}
		endStep();
		beginStep("[2] EPESI - Dashboard (/#2)", 0);
		{
			web.window(9, "{{obj.Epesi_CadUsuarioL.web_window_0_1}}").waitForPage(null);
			{
				think(2.63);
			}
			web.element(10, "{{obj.Epesi_CadUsuarioL.web_span_Contacts}}").click();
		}
		endStep();
		beginStep("[3] EPESI - Contacts: Browse (/#3)", 0);
		{
			web.window(11, "{{obj.Epesi_CadUsuarioL.web_window_0_2}}").waitForPage(null);
			{
				think(1.48);
			}
			web.image(12, "{{obj.Epesi_CadUsuarioL.web_img_38}}").click();
		}
		endStep();
		beginStep("[4] EPESI - Contacts: New record (/#4)", 0);
		{
			web.window(13, "{{obj.Epesi_CadUsuarioL.web_window_0_3}}").waitForPage(null);
			{
				think(1.848);
			}
			web.textBox(14,"{{obj.Epesi_CadUsuarioL.web_input_text_last_name}}").setText("Damasio");
			{
				think(0.233);
			}
			web.textBox(15,"{{obj.Epesi_CadUsuarioL.web_input_text_last_name}}").pressTab();
			{
				think(0.759);
			}
			web.textBox(16,"{{obj.Epesi_CadUsuarioL.web_input_text_first_name}}").setText("Juliana");
			{
				think(6.671);
			}
			web.selectBox(17, "{{obj.Epesi_CadUsuarioL.web_select_permission}}").selectOptionByText("Public, Read-Only");
			{
				think(6.12);
			}
			web.element(18, "{{obj.Epesi_CadUsuarioL.web_span_Save}}").click();
		}
		endStep();
		beginStep("[5] EPESI - Contacts: View record (/#5)", 0);
		{
			web.window(19, "{{obj.Epesi_CadUsuarioL.web_window_0_4}}").waitForPage(null);
		}
		endStep();
	}
	
	public void finish() throws Exception 
	{

	}
}
