using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents all the Object-Action pairs available to the OATS Framework.
    /// </summary>
    public class KeywordsTable
        : IComparable, ICloneable
    {
        #region Fields and Properties
        private List<KeywordValuePair> table;

        public List<KeywordValuePair> Table
        {
            get { return table; }
            set { table = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for KeywordTable. Does not set fields.
        /// </summary>
        public KeywordsTable()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordsTable with the specified table's values.
        /// </summary>
        /// <param name="table">Table from which to take the values</param>
        public KeywordsTable(List<KeywordValuePair> table)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordsTable as a close of the given parameter.
        /// </summary>
        /// <param name="value">KeywordsTable to clone</param>
        internal KeywordsTable(KeywordsTable value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializes an instance of KeywordsTable with the given parameters.
        /// </summary>
        /// <param name="table">Table from which to take the values</param>
        private void Initialize(List<KeywordValuePair> table)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Add Operations
        /// <summary>
        /// Adds a new KeywordValuePair with the specified parameters.
        /// </summary>
        /// <param name="kObject">KeywordObject value</param>
        /// <param name="kAction">KeywordAction value</param>
        /// <returns>False if Object-Action pair already exists in table, True otherwise</returns>
        public bool Add(KeywordObject kObject, KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a new KeywordValuePair with the specified parameters.
        /// </summary>
        /// <param name="kObject">KeywordObject value</param>
        /// <param name="kAction">KeywordAction value</param>
        /// <param name="keyword">Keyword value</param>
        /// <returns>False if Object-Action pair already exists in table, True otherwise</returns>
        public bool Add(KeywordObject kObject, KeywordAction kAction, Keyword keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a new KeywordValuePair with the specified parameters.
        /// </summary>
        /// <param name="kObject">KeywordObject value</param>
        /// <param name="kAction">KeywordAction value</param>
        /// <param name="keyword">Keyword value</param>
        /// <param name="srm">StepRunMode value</param>
        /// <returns>False if Object-Action pair already exists in table, True otherwise</returns>
        public bool Add(KeywordObject kObject, KeywordAction kAction, Keyword keyword, StepRunMode srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a new KeywordValuePair with the specified parameters.
        /// </summary>
        /// <param name="kObject">KeywordObject value</param>
        /// <param name="kAction">KeywordAction value</param>
        /// <returns>False if Object-Action pair already exists in table, True otherwise</returns>
        public bool Add(String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a new KeywordValuePair with the specified parameters.
        /// </summary>
        /// <param name="kObject">KeywordObject value</param>
        /// <param name="kAction">KeywordAction value</param>
        /// <param name="keyword">Keyword value</param>
        /// <returns>False if Object-Action pair already exists in table, True otherwise</returns>
        public bool Add(String kObject, String kAction, String keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds a new KeywordValuePair with the specified parameters.
        /// </summary>
        /// <param name="kObject">KeywordObject value</param>
        /// <param name="kAction">KeywordAction value</param>
        /// <param name="keyword">Keyword value</param>
        /// <param name="srm">StepRunMode value</param>
        /// <returns>False if Object-Action pair already exists in table, True otherwise</returns>
        public bool Add(String kObject, String kAction, String keyword, String srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds the given KeywordValuePair.
        /// </summary>
        /// <param name="value">KeywordValuePair to be added</param>
        /// <returns>False if Object-Action pair already exists in table, True otherwise</returns>
        public bool Add(KeywordValuePair value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Remove Operations
        /// <summary>
        /// Removes all Object-Action pairs that contain the given KeywordObject.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <returns>True if any values are removed, False otherwise</returns>
        public bool Remove(KeywordObject kObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes all Object-Action pairs that contain the given KeywordAction.
        /// </summary>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>True if any values are removed, False otherwise</returns>
        public bool Remove(KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the specified Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>True if value is removed, False otherwise</returns>
        public bool Remove(KeywordObject kObject, KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the specified Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>True if value is removed, False otherwise</returns>
        public bool Remove(String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes all Object-Action pairs that contain the given Keyword.
        /// </summary>
        /// <param name="keyword">Keyword to search for</param>
        /// <returns>True if any values are removed, False otherwise</returns>
        public bool Remove(Keyword keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes all Object-Action pairs that contain the given StepRunMode.
        /// </summary>
        /// <param name="srm">StepRunMode to search for</param>
        /// <returns>True if any values are removed, False otherwise</returns>
        public bool Remove(StepRunMode srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the specified KeywordValuePair object.
        /// </summary>
        /// <param name="value">KeywordValuePair object to remove</param>
        /// <returns>True if value is removed, False otherwise</returns>
        public bool Remove(KeywordValuePair value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes all objects present in the given table.
        /// </summary>
        /// <param name="table">Table of objects to remove</param>
        /// <returns>True if any values are removed, False otherwise</returns>
        public bool Remove(KeywordsTable table)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes all objects present in the given table.
        /// </summary>
        /// <param name="table">Table of objects to remove</param>
        /// <returns>True if any values are removed, False otherwise</returns>
        public bool Remove(List<KeywordValuePair> table)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Exists Operations
        /// <summary>
        /// Checks for the existance of the specified KeywordObject.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <returns>True if the given object is present in the set, false otherwise</returns>
        public bool Exists(KeywordObject kObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Checks for the existance of the specified KeywordAction.
        /// </summary>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>True if the given object is present in the set, false otherwise</returns>
        public bool Exists(KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Checks for the existance of the specified Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>True if the given object is present in the set, false otherwise</returns>
        public bool Exists(KeywordObject kObject, KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Checks for the existance of the specified Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>True if the given object is present in the set, false otherwise</returns>
        public bool Exists(String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Checks for the existance of the specified Keyword.
        /// </summary>
        /// <param name="keyword">Keyword to search for</param>
        /// <returns>True if the given object is present in the set, false otherwise</returns>
        public bool Exists(Keyword keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Checks for the existance of the specified StepRunMode.
        /// </summary>
        /// <param name="srm">StepRunMode to search for</param>
        /// <returns>True if the given object is present in the set, false otherwise</returns>
        public bool Exists(StepRunMode srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Checks for the existance of the specified KeywordValuePair.
        /// </summary>
        /// <param name="value">KeywordValuePair to search for</param>
        /// <returns>True if the given object is present in the set, false otherwise</returns>
        public bool Exists(KeywordValuePair value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Get Operations
        /// <summary>
        /// Returns the KeywordValuePair object referrent to the given Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject of the Object-Action pair</param>
        /// <param name="kAction">KeywordAction of the Object-Action pair</param>
        /// <returns>KeywordValuePair object referrent to the given Object-Action pair</returns>
        public KeywordValuePair GetValue(KeywordObject kObject, KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns the KeywordValuePair object referrent to the given Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject of the Object-Action pair</param>
        /// <param name="kAction">KeywordAction of the Object-Action pair</param>
        /// <returns>KeywordValuePair object referrent to the given Object-Action pair</returns>
        public KeywordValuePair GetValue(String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given KeywordObject.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValues(KeywordObject kObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given KeywordAction.
        /// </summary>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValues(KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given Keyword.
        /// </summary>
        /// <param name="keyword">Keyword to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValues(Keyword keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given StepRunMode.
        /// </summary>
        /// <param name="srm">StepRunMode to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValues(StepRunMode srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given KeywordObject.
        /// </summary>
        /// <param name="kObject">KeywordObject to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValuesByObject(String kObject)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given KeywordAction.
        /// </summary>
        /// <param name="kAction">KeywordAction to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValuesByAction(String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given Keyword.
        /// </summary>
        /// <param name="keyword">Keyword to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValuesByKeyword(String keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns a table containing all KeywordValuePairs that make reference to the given StepRunMode.
        /// </summary>
        /// <param name="srm">StepRunMode to search for</param>
        /// <returns>Table containing all objects related to the parameter</returns>
        public KeywordsTable GetValuesByStepRunMode(String srm)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Auxiliary Methods
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Checks if all KeywordValuePair objects present in one KeywordsTable are also present in another, regardless of order.
        /// </summary>
        /// <param name="obj">KeywordsTable instance to compare with</param>
        /// <returns>True if sets are equal, False otherwise</returns>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
        public object Clone()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
