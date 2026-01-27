namespace Behaviour_Tree.Blackboard
{
    public static class StringExtensions
    {
        /// <summary>
        /// Computes the FNV-1a hash for the input string.
        /// The FNV-a1 hash is a non-cryptographic hash function known for its speed and good distribution properties.
        /// Usefil for creating Dictionary keys instead of using strings.
        /// </summary>
        /// <param name="str"></param>
        /// <returns> An interger representing the FNV-1a hash of the input string </returns>
        public static int ComputeFNV1aHash(this string str)
        {
            uint hash = 2166136261;
            foreach (char c in str)
            {
                hash = (hash ^ c) * 16777619; 
            }
            return unchecked((int)hash);
        }
    
    }
}
