using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Stores information on all known relationships between OATS scripts and the OATS Framework.
    /// </summary>
    public class OgmaParameterTable
    {
        #region Fields and Properties
        private ScriptValuesTable scriptValues;
        private KeywordsTable keywordValues;
        private List<Keyword> keywords;
        private List<StepRunMode> stepRunModes;
        private List<ScriptObject> scriptObjects;

        public ScriptValuesTable ScriptValues
        {
            get { return scriptValues; }
            set { scriptValues = value; }
        }
        public KeywordsTable KeywordValues
        {
            get { return keywordValues; }
            set { keywordValues = value; }
        }
        public List<Keyword> Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }
        public List<StepRunMode> StepRunModes
        {
            get { return stepRunModes; }
            set { stepRunModes = value; }
        }
        public List<ScriptObject> ScriptObjects
        {
            get { return scriptObjects; }
            set { scriptObjects = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor. Sets initial variables.
        /// </summary>
        public OgmaParameterTable()
        {
            Initialize();
        }
        /// <summary>
        /// Extended constructor. Sets initial variables and loads the Keyword Values.
        /// </summary>
        public OgmaParameterTable(Dictionary<Tuple<String, String>, String> values)
        {
            Initialize();
            LoadKeywordValues(values);
        }
        private void Initialize()
        {
            ScriptValues = new ScriptValuesTable();
            KeywordValues = new KeywordsTable();
            Keywords = new List<Keyword>();
            StepRunModes = new List<StepRunMode>();
        }
        #endregion

        #region Load and Overwrite Methods
        #region KeywordValues
        /// <summary>
        /// Loads the Keyword Values to their initial state based on a dictionary containing the information from the Excel table.
        /// 
        /// WARNING: This method DOES NOT reset the Keywords table. If you mean to reset it, use OverwriteKeywordValues instead.
        /// </summary>
        public void LoadKeywordValues(Dictionary<Tuple<String, String>, String> values)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overwrites the Keyword Values, removing all present values and filling them in with a new list of values.
        /// 
        /// WARNING: This method is probably useless and likely to destroy everything. User discretion is advised.
        /// </summary>
        public void OverwriteKeywordValues(Dictionary<Tuple<String, String>, String> values)
        {
            KeywordValues = new KeywordsTable();
            LoadKeywordValues(values);
        }
        /// <summary>
        /// Completes an existing Keyword Values table by adding all pairs from the new list that were not already present.
        /// 
        /// WARNING: This method DOES NOT update pairs that were already present. For that purpose, use UpdateKeywordValues.
        /// </summary>
        public void CompleteKeywordValues(Dictionary<Tuple<String, String>, String> values)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Updates the existing Keyword values table, adding all pairs not present and modifying those present.
        /// 
        /// WARNING: This method DOES NOT discriminate, and is very likely to break things if used lightly.
        /// </summary>
        public void UpdateKeywordValues(Dictionary<Tuple<String, String>, String> values)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Add and Remove Methods
        #region KeywordTable and StepRunMode
        /// <summary>
        /// Adds a new Object-Action value pair to the Keywords table. Returns false if the pair already exists.
        /// </summary>
        /// <param name="kObject">sdf</param>
        /// <param name="kAction">sdf</param>
        /// <returns>sdf</returns>
        public bool AddKeywordValuePair(String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes an Object-Action value pair from the Keywords table. Returns false if the pair is not found.
        /// </summary>
        public bool RemoveKeywordValuePair(String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes all Object-Action value pairs related to the given Object value from the Keywords table. Returns false if none are found.
        /// </summary>
        public bool RemoveKeywordObject(String kObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a Keyword value to the available Keywords list. Returns false if the keyword already exists.
        /// </summary>
        public bool AddKeyword(String keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes a Keyword value from the available Keywords list. Returns false if the keyword is not found.
        /// </summary>
        public bool RemoveKeyword(String keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a new StepRunMode value to the available list. Returns false if the value already exists.
        /// </summary>
        public bool AddStepRunMode(String srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes a StepRunMode value from the available list. Returns false if the value is not found.
        /// </summary>
        public bool RemoveStepRunMode(String srm)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region ScriptTable
        /// <summary>
        /// Adds a Script Object to the list of available Script Objects. Does not set a relationship with a Keyword Object. Returns false if the Script Object already exists.
        /// </summary>
        public bool AddScriptObject(String sObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a Script Object to the list of available Script Objects and sets its related Keyword Object. Returns -1 if the Keyword Object does not exist, 0 if the Script Object already exists, 1 if added successfully.
        /// </summary>
        public int AddScriptObject(String sObject, String kObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes a Script Object from the list of available Script Objects. Returns false if the Script Object is not found.
        /// </summary>
        public bool RemoveScriptObject(String sObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a new Object-Action value pair to the Script table. Returns false if the pair already exists. If the Script Object assigned does not currently exist, it will be added automatically.
        /// </summary>
        public bool AddScriptValuePair(String sObject, String sAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes an Object-Action value pair from the Script table. Returns false if the pair is not found.
        /// </summary>
        public bool RemoveScriptValuePair(String sObject, String sAction)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Update and Overwrite Methods
        #region KeywordTable and StepRunMode
        #region Direct Methods
        /// <summary>
        /// Sets a Keyword value to a KeywordValuePair. Returns false if the KeywordValuePair already has a Keyword value. To overwrite, use OverwriteKeywordToValuePair.
        /// </summary>
        public bool SetKeywordToValuePair(Keyword value, KeywordValuePair kvp)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overwrites the Keyword value of a KeywordValuePair. Returns false if the KeywordValuePair did not have a previous Keyword value.
        /// </summary>
        public bool OverwriteKeywordToValuePair(Keyword value, KeywordValuePair kvp)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets a StepRunMode value to a KeywordValuePair. Returns false if the KeywordValuePair already has a StepRunMode value. To overwrite, use OverwriteStepRunModeToValuePair.
        /// </summary>
        public bool SetStepRunModeToValuePair(StepRunMode value, KeywordValuePair kvp)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overwrites the StepRunMode value of a KeywordValuePair. Returns false if the KeywordValuePair did not have a previous StepRunMode value.
        /// </summary>
        public bool OverwriteStepRunModeToValuePair(StepRunMode value, KeywordValuePair kvp)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets a StepRunMode value to a Keyword. Returns false if the Keyword already has a StepRunMode value. To overwrite, use OverwriteStepRunModeToKeyword.
        /// </summary>
        public bool SetStepRunModeToKeyword(StepRunMode value, Keyword key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overwrites the StepRunMode value of a KeywordValuePair. Returns false if the KeywordValuePair did not have a previous StepRunMode value.
        /// </summary>
        public bool OverwriteStepRunModeToKeyword(StepRunMode value, Keyword key)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Proxy Methods
        /// <summary>
        /// Sets a Keyword value to a KeywordValuePair. Returns false if the KeywordValuePair already has a Keyword value. To overwrite, use OverwriteKeywordToValuePair.
        /// </summary>
        public bool SetKeywordToValuePair(String keyword, String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overwrites the Keyword value of a KeywordValuePair. Returns false if the KeywordValuePair did not have a previous Keyword value.
        /// </summary>
        public bool OverwriteKeywordToValuePair(String keyword, String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets a StepRunMode value to a KeywordValuePair. Returns false if the KeywordValuePair already has a StepRunMode value. To overwrite, use OverwriteStepRunModeToValuePair.
        /// </summary>
        public bool SetStepRunModeToValuePair(String srm, String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overwrites the StepRunMode value of a KeywordValuePair. Returns false if the KeywordValuePair did not have a previous StepRunMode value.
        /// </summary>
        public bool OverwriteStepRunModeToValuePair(String srm, String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets a StepRunMode value to a Keyword. Returns false if the Keyword already has a StepRunMode value. To overwrite, use OverwriteStepRunModeToKeyword.
        /// </summary>
        public bool SetStepRunModeToKeyword(String srm, String keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overwrites the StepRunMode value of a KeywordValuePair. Returns false if the KeywordValuePair did not have a previous StepRunMode value.
        /// </summary>
        public bool OverwriteStepRunModeToKeyword(String srm, String keyword)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        #region ScriptTable
        /// <summary>
        /// Overwrites the Keyword Object related to the given Script Object. Returns -1 if Script Object is not found, 0 if no Keyword Object was previously related, 1 if the operation concludes normally.
        /// </summary>
        public int UpdateScriptObject(String sObject, String kObject)
        {
            return OverwriteScriptObject(sObject, kObject);
        }
        /// <summary>
        /// Overwrites the Keyword Object related to the given Script Object. Returns -1 if Script Object is not found, 0 if no Keyword Object was previously related, 1 if the operation concludes normally.
        /// </summary>
        public int OverwriteScriptObject(String sObject, String kObject)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
