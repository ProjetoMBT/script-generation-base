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
		beginStep("[1] Moodle (/moodle/)", 0);
		{
			web.window(2, "{{obj.moodleL.web_window_0}}").waitForPage(null);
			{
				think(3.473);
			}
			web.link(4, "{{obj.moodleL.web_a_Login_here_}}").click();
		}
		endStep();
		beginStep("[2] Moodle: Login to the site (/login/)", 0);
		{
			web.window(5, "{{obj.moodleL.web_window_0_1}}").waitForPage(null);
			{
				think(1.291);
			}
			web.textBox(6, "{{obj.moodleL.web_input_text_username}}").click();
			{
				think(1.614);
			}
			web.textBox(7, "{{obj.moodleL.web_input_text_username}}").setText(
					"lcosta");
			{
				think(0.337);
			}
			web.textBox(8, "{{obj.moodleL.web_input_text_username}}")
					.pressTab();
			{
				think(0.586);
			}
			web.textBox(9, "{{obj.moodleL.web_input_password_password}}")
					.setPassword(deobfuscate("+7Qav0cxWTQA0OpbEx8+dw=="));
			{
				think(1.689);
			}
			web.button(10, "{{obj.moodleL.web_input_submit_loginbtn}}").click();
		}
		endStep();
		beginStep("[3] Moodle (/moodle/)", 0);
		{
			web.window(11, "{{obj.moodleL.web_window_0}}").waitForPage(null);
			{
				think(3.981);
			}
			web.link(12, "{{obj.moodleL.web_a_My_courses}}").click();
		}
		endStep();
		beginStep("[4] Moodle: My home (/my/)", 0);
		{
			web.window(13, "{{obj.moodleL.web_window_0_2}}").waitForPage(null);
			{
				think(1.629);
			}
			web.link(14, "{{obj.moodleL.web_a_Functional_Testing}}").click();
		}
		endStep();
		beginStep("[5] Course: Functional Testing (/view.php)", 0);
		{
			web.window(15, "{{obj.moodleL.web_window_0_3}}").waitForPage(null);
			{
				think(3.522);
			}
			web.link(16, "{{obj.moodleL.web_a_Logout}}").click();
		}
		endStep();
		beginStep("[6] Moodle (/moodle/)", 0);
		{
			web.window(17, "{{obj.moodleL.web_window_0}}").waitForPage(null);
		}
		endStep();

	}
	
	public void finish() throws Exception {
	}
}
