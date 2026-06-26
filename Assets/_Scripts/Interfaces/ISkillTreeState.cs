using System.Collections.Generic;

namespace Interfaces
{
    /// <summary>
    /// Abstraction over the skill-tree presentation. Lets the persistence layer capture and restore
    /// which skill-tree nodes are unlocked without referencing concrete UI types — the UI implements
    /// this and registers it with the <c>ServiceLocator</c>, inverting the old player → UI dependency.
    /// </summary>
    public interface ISkillTreeState
    {
        /// <summary>Returns the data key (node name) of every currently unlocked skill-tree node.</summary>
        /// <returns>The unlocked node keys.</returns>
        List<string> GetUnlockedNodeKeys();

        /// <summary>Resets every node, then restores the unlocked state for the supplied node keys.</summary>
        /// <param name="nodeKeys">Keys of the nodes that should be unlocked.</param>
        void RestoreUnlockedNodes(IEnumerable<string> nodeKeys);
    }
}
