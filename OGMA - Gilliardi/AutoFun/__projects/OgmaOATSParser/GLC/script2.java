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
			web.window(2, "{{obj.epesiB.web_window_0}}").waitForPage(null);
			{
				think(2.738);
			}
			web.textBox(4, "{{obj.epesiB.web_input_text_username}}").setText(
					"admin");
			{
				think(0.225);
			}
			web.textBox(5, "{{obj.epesiB.web_input_text_username}}").pressTab();
			{
				think(0.684);
			}
			web.textBox(6, "{{obj.epesiB.web_input_password_password}}")
					.setPassword(deobfuscate("CzMwzF+2+fwXkAwc6LSY/g=="));
			{
				think(0.531);
			}
			web.textBox(7, "{{obj.epesiB.web_input_password_password}}")
					.pressEnter();
		}
		endStep();
		beginStep("[2] EPESI - Dashboard (/#2)", 0);
		{
			web.window(8, "{{obj.epesiB.web_window_0_1}}").waitForPage(null);
			{
				think(2.486);
			}
			web.element(9, "{{obj.epesiB.web_span_Calendar}}").click();
		}
		endStep();
		beginStep("[3] EPESI - Calendar (/#3)", 0);
		{
			web.window(10, "{{obj.epesiB.web_window_0_2}}").waitForPage(null);
			{
				think(2.751);
			}
			web.element(11, "{{obj.epesiB.web_td__}}").click();
			{
				think(3.344);
			}
			web.element(12, "{{obj.epesiB.web_td__}}").click();
			{
				think(0.031);
			}
			web.element(13, "{{obj.epesiB.web_td___1}}").dblClick();
			{
				think(3.994);
			}
			web.element(14, "{{obj.epesiB.web_span_Phonecall}}").click();
		}
		endStep();
		beginStep("[4] EPESI - Phonecalls: New record (/#4)", 0);
		{
			web.window(15, "{{obj.epesiB.web_window_0_3}}").waitForPage(null);
		}
		endStep();

	}
	
	public void finish() throws Exception {
	}
}
