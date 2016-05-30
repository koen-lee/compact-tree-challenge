namespace Trie
{
    //  https://ayende.com/blog/174049/the-low-level-interview-question
    public interface ITrie
    {
        // Try to add the key/val to the trie
        // can fail if there isn't enough room
        // if already exists, will overwrite old 
        // value
        bool TryWrite(string key, long value);

        // Try to find the key in the trie, if found,
        // will put the value in the out param.
        // Can fail if value is not there
        bool TryRead(string key, out long value);

        // Remove the key from the trie, noop
        // if the key isn't there
        void Delete(string key);

        // Saves the internal array to a file
        void Save(string filename);

        // Loads the internal array from a file
        void Load(string filename);
    }
}