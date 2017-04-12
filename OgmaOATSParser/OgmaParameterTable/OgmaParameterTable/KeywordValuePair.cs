using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents an Object-Action pair, as presented in the OATS Framework.
    /// </summary>
    public class KeywordValuePair
        : IComparable, ICloneable
    {
        #region Fields and Properties
        private KeywordObject kObject;
        private KeywordAction kAction;
        private Keyword keyword;
        private StepRunMode stepRunMode;

        public KeywordObject KObject
        {
            get { return kObject; }
            set { kObject = value; }
        }
        public KeywordAction KAction
        {
            get { return kAction; }
            set { kAction = value; }
        }
        public Keyword Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }
        public StepRunMode StepRunMode
        {
            get { return stepRunMode; }
            set { stepRunMode = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for Keyword. Does not set fields.
        /// </summary>
        internal KeywordValuePair()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordValuePair with the specified Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject instance</param>
        /// <param name="kAction">KeywordAction instance</param>
        public KeywordValuePair(KeywordObject kObject, KeywordAction kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordValuePair with the specified Object-Action pair and Keyword.
        /// </summary>
        /// <param name="kObject">KeywordObject instance</param>
        /// <param name="kAction">KeywordAction instance</param>
        /// <param name="keyword">Keyword instance</param>
        public KeywordValuePair(KeywordObject kObject, KeywordAction kAction, Keyword keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordValuePair with the specified Object-Action pair, Keyword and StepRunMode.
        /// </summary>
        /// <param name="kObject">KeywordObject instance</param>
        /// <param name="kAction">KeywordAction instance</param>
        /// <param name="keyword">Keyword instance</param>
        /// <param name="srm">StepRunMode instance</param>
        public KeywordValuePair(KeywordObject kObject, KeywordAction kAction, Keyword keyword, StepRunMode srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordValuePair with the specified Object-Action pair.
        /// </summary>
        /// <param name="kObject">KeywordObject name</param>
        /// <param name="kAction">KeywordAction name</param>
        public KeywordValuePair(String kObject, String kAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordValuePair with the specified Object-Action pair and Keyword.
        /// </summary>
        /// <param name="kObject">KeywordObject name</param>
        /// <param name="kAction">KeywordAction name</param>
        /// <param name="keyword">Keyword name</param>
        public KeywordValuePair(String kObject, String kAction, String keyword)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordValuePair with the specified Object-Action pair, Keyword and StepRunMode.
        /// </summary>
        /// <param name="kObject">KeywordObject name</param>
        /// <param name="kAction">KeywordAction name</param>
        /// <param name="keyword">Keyword name</param>
        /// <param name="srm">StepRunMode name</param>
        public KeywordValuePair(String kObject, String kAction, String keyword, String srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of KeywordValuePair as a close of the given parameter.
        /// </summary>
        /// <param name="value">KeywordValuePair to clone</param>
        internal KeywordValuePair(KeywordValuePair value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializes an instance of KeywordValuePair with the given parameters.
        /// </summary>
        /// <param name="kObject">KeywordObject name</param>
        /// <param name="kAction">KeywordAction name</param>
        /// <param name="keyword">Keyword name</param>
        /// <param name="srm">StepRunMode name</param>
        private void Initialize(String kObject, String kAction, String keyword, String srm)
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
        /// Compares the Object-Action pair of this KeywordValuePair instance to the Object-Action pair of the given KeywordValuePair instance.
        /// </summary>
        /// <param name="obj">KeywordValuePair instance to compare with</param>
        /// <returns>True if Object-Action pairs are equal, False if Object-Action pairs are different</returns>
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
