/**
 * Java implementation of Python's pickle serialization protocol.
 * 
 * The {@link net.razorvine.pickle.Unpickler} supports the full pickle protocol (level 0,1,2,3).
 * The {@link net.razorvine.pickle.Pickler} supports most of the protocol (level 2 only though).
 * 
 * Python's data types are mapped on their Java equivalents and vice versa.
 * Most basic data types and container types are supported by default.
 * You can add custom object pickle and unpickle classes to extend this
 * functionality.
 * 
 * @author Irmen de Jong (irmen@razorvine.net)
 * @version 1.10
 */
package net.razorvine.pickle;

