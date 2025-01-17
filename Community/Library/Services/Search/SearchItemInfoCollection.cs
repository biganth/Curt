#region Copyright
// 
// DotNetNukeŽ - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System;
using System.Collections;

#endregion

namespace DotNetNuke.Services.Search
{
    /// -----------------------------------------------------------------------------
    /// Namespace:  DotNetNuke.Services.Search
    /// Project:    DotNetNuke
    /// Class:      SearchItemInfoCollection
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Represents a collection of <see cref="SearchItemInfo">SearchItemInfo</see> objects.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class SearchItemInfoCollection : CollectionBase
    {
		#region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> class.
        /// </summary>
        public SearchItemInfoCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> class containing the elements of the specified source collection.
        /// </summary>
        /// <param name="value">A <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> with which to initialize the collection.</param>
        public SearchItemInfoCollection(SearchItemInfoCollection value)
        {
            AddRange(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> class containing the specified array of <see cref="SearchItemInfo">SearchItemInfo</see> objects.
        /// </summary>
        /// <param name="value">An array of <see cref="SearchItemInfo">SearchItemInfo</see> objects with which to initialize the collection. </param>
        public SearchItemInfoCollection(SearchItemInfo[] value)
        {
            AddRange(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchItemInfoCollection">SearchItemInfoCollectionSearchItemInfoCollection</see> class containing the specified array of <see cref="SearchItemInfo">SearchItemInfo</see> objects.
        /// </summary>
        /// <param name="value">An arraylist of <see cref="SearchItemInfo">SearchItemInfo</see> objects with which to initialize the collection. </param>
        public SearchItemInfoCollection(ArrayList value)
        {
            AddRange(value);
        }

		#endregion

		#region "Properties"

        /// <summary>
        /// Gets the <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> at the specified index in the collection.
        /// <para>
        /// In VB.Net, this property is the indexer for the <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> class.
        /// </para>
        /// </summary>
        public SearchItemInfo this[int index]
        {
            get
            {
                return (SearchItemInfo) List[index];
            }
            set
            {
                List[index] = value;
            }
        }

		#endregion

		#region "Public Methods"

        /// <summary>
        /// Add an element of the specified <see cref="SearchItemInfo">SearchItemInfo</see> to the end of the collection.
        /// </summary>
        /// <param name="value">An object of type <see cref="SearchItemInfo">SearchItemInfo</see> to add to the collection.</param>
        public int Add(SearchItemInfo value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Gets the index in the collection of the specified <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see>, if it exists in the collection.
        /// </summary>
        /// <param name="value">The <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> to locate in the collection.</param>
        /// <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        public int IndexOf(SearchItemInfo value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Add an element of the specified <see cref="SearchItemInfo">SearchItemInfo</see> to the collection at the designated index.
        /// </summary>
        /// <param name="index">An <see cref="System.Int32">Integer</see> to indicate the location to add the object to the collection.</param>
        /// <param name="value">An object of type <see cref="SearchItemInfo">SearchItemInfo</see> to add to the collection.</param>
        public void Insert(int index, SearchItemInfo value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Remove the specified object of type <see cref="SearchItemInfo">SearchItemInfo</see> from the collection.
        /// </summary>
        /// <param name="value">An object of type <see cref="SearchItemInfo">SearchItemInfo</see> to remove to the collection.</param>
        public void Remove(SearchItemInfo value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the specified <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see>.
        /// </summary>
        /// <param name="value">The <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> to search for in the collection.</param>
        /// <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        public bool Contains(SearchItemInfo value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Copies the elements of the specified <see cref="SearchItemInfo">SearchItemInfo</see> array to the end of the collection.
        /// </summary>
        /// <param name="value">An array of type <see cref="SearchItemInfo">SearchItemInfo</see> containing the objects to add to the collection.</param>
        public void AddRange(SearchItemInfo[] value)
        {
            for (int i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }

        /// <summary>
        /// Copies the elements of the specified arraylist to the end of the collection.
        /// </summary>
        /// <param name="value">An arraylist of type <see cref="SearchItemInfo">SearchItemInfo</see> containing the objects to add to the collection.</param>
        public void AddRange(ArrayList value)
        {
            foreach (object obj in value)
            {
                if (obj is SearchItemInfo)
                {
                    Add((SearchItemInfo) obj);
                }
            }
        }

        /// <summary>
        /// Adds the contents of another <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> to the end of the collection.
        /// </summary>
        /// <param name="value">A <see cref="SearchItemInfoCollection">SearchItemInfoCollection</see> containing the objects to add to the collection. </param>
        public void AddRange(SearchItemInfoCollection value)
        {
            for (int i = 0; i <= value.Count - 1; i++)
            {
                Add((SearchItemInfo) value.List[i]);
            }
        }

        /// <summary>
        /// Copies the collection objects to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
        /// <param name="index">The index of the array at which to begin inserting.</param>
        public void CopyTo(SearchItemInfo[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        /// Creates a one-dimensional <see cref="T:System.Array">Array</see> instance containing the collection items.
        /// </summary>
        /// <returns>Array of type SearchItemInfo</returns>
        public SearchItemInfo[] ToArray()
        {
            var arr = new SearchItemInfo[Count];
            CopyTo(arr, 0);

            return arr;
        }

        public SearchItemInfoCollection ModuleItems(int ModuleId)
        {
            var retValue = new SearchItemInfoCollection();
            foreach (SearchItemInfo info in this)
            {
                if (info.ModuleId == ModuleId)
                {
                    retValue.Add(info);
                }
            }
            return retValue;
        }
		
		#endregion
    }
}
