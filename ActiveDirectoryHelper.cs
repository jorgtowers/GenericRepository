using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.Configuration;


namespace Utils.ActiveDirectory
{
    public class ActiveDirectoryHelper
    {
        private DirectoryEntry _directoryEntry = null;
        private string _LDAPPath, _LDAPUser, _LDAPPassword, _LDAPDomain;
        //<add key="LDAPPassword" value="xxxPasswordxxx " />
        //<add key="LDAPPath" value="LDAP://domain/DC=xxx,DC=com" />
        //<add key="LDAPUser" value="xxxUserxxx " />

        private DirectoryEntry SearchRoot
        {
            get
            {
                if (_directoryEntry == null)
                {
                    _directoryEntry = new DirectoryEntry(LDAPPath, LDAPUser, LDAPPassword, AuthenticationTypes.Secure);
                }
                return _directoryEntry;
            }
        }

        public String LDAPPath
        {
            get
            {
                _LDAPPath = "LDAP://" + _LDAPDomain + "/DC=" + _LDAPDomain.Substring(0, _LDAPDomain.LastIndexOf(".")) + ",DC=" + _LDAPDomain.Substring(_LDAPDomain.LastIndexOf(".")+1);
                return _LDAPPath;
            }
        }

        public String LDAPUser
        {
            get
            {
                return _LDAPUser;
            }
            set { _LDAPUser = value; }
        }

        public String LDAPPassword
        {
            get
            {
                return _LDAPPassword;
            }
            set { _LDAPPassword = value; }
        }

        public String LDAPDomain
        {
            get
            {
                return _LDAPDomain;
            }
            set { _LDAPDomain = value; }
        }

        internal ADUserDetail GetUserByFullName(String userName)
        {
            try
            {
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Filter = "(&(objectClass=user)(cn=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path, LDAPUser, LDAPPassword);
                    return ADUserDetail.GetUser(user);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ADUserDetail GetUserByLoginName(String userName)
        {
            try
            {
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Filter = "(&(objectClass=user)(SAMAccountName=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path, LDAPUser, LDAPPassword);
                    return ADUserDetail.GetUser(user);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// This function will take a DL or Group name and return list of users
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public List<ADUserDetail> GetUserFromGroup(String groupName)
        {
            List<ADUserDetail> userlist = new List<ADUserDetail>();
            try
            {
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + groupName + "))";
                SearchResult results = directorySearch.FindOne();
                if (results != null)
                {

                    DirectoryEntry deGroup = new DirectoryEntry(results.Path, LDAPUser, LDAPPassword);
                    System.DirectoryServices.PropertyCollection pColl = deGroup.Properties;
                    int count = pColl["member"].Count;


                    for (int i = 0; i < count; i++)
                    {
                        string respath = results.Path;
                        string[] pathnavigate = respath.Split("CN".ToCharArray());
                        respath = pathnavigate[0];
                        string objpath = pColl["member"][i].ToString();
                        string path = respath + objpath;


                        DirectoryEntry user = new DirectoryEntry(path, LDAPUser, LDAPPassword);
                        ADUserDetail userobj = ADUserDetail.GetUser(user);
                        userlist.Add(userobj);
                        user.Close();
                    }
                }
                return userlist;
            }
            catch (Exception ex)
            {
                return userlist;
            }

        }

        #region Get user with First Name

        public List<ADUserDetail> GetUsersByFirstName(string fName)
        {

            //UserProfile user;
            List<ADUserDetail> userlist = new List<ADUserDetail>();
            string filter = "";

            _directoryEntry = null;
            DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
            directorySearch.Asynchronous = true;
            directorySearch.CacheResults = true;
            filter = string.Format("(givenName={0}*", fName);
            //            filter = "(&(objectClass=user)(objectCategory=person)(givenName="+fName+ "*))";


            directorySearch.Filter = filter;

            SearchResultCollection userCollection = directorySearch.FindAll();
            foreach (SearchResult users in userCollection)
            {
                DirectoryEntry userEntry = new DirectoryEntry(users.Path, LDAPUser, LDAPPassword);
                ADUserDetail userInfo = ADUserDetail.GetUser(userEntry);

                userlist.Add(userInfo);

            }

            directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + fName + "*))";
            SearchResultCollection results = directorySearch.FindAll();
            if (results != null)
            {

                foreach (SearchResult r in results)
                {
                    DirectoryEntry deGroup = new DirectoryEntry(r.Path, LDAPUser, LDAPPassword);

                    ADUserDetail agroup = ADUserDetail.GetUser(deGroup);
                    userlist.Add(agroup);
                }

            }
            return userlist;
        }

        #endregion


        #region AddUserToGroup
        public bool AddUserToGroup(string userlogin, string groupName)
        {
            try
            {
                _directoryEntry = null;
                ADManager admanager = new ADManager(LDAPDomain, LDAPUser, LDAPPassword);
                admanager.AddUserToGroup(userlogin, groupName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region RemoveUserToGroup
        public bool RemoveUserToGroup(string userlogin, string groupName)
        {
            try
            {
                _directoryEntry = null;
                ADManager admanager = new ADManager("xxx", LDAPUser, LDAPPassword);
                admanager.RemoveUserFromGroup(userlogin, groupName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }

}
