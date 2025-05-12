// SPDX-License-Identifier: MIT
pragma solidity ^0.8.19;

/// @title SimpleStorage
/// @notice A simple contract for storing and retrieving a single uint value
/// @dev This is an improved version of the original SimpleStorage contract
contract SimpleStorage {
    // State variable to store the data
    uint private storedData;
    
    // Event emitted when the stored value is changed
    event ValueChanged(address indexed sender, uint newValue);
    
    /// @notice Sets the value of the stored data
    /// @param x The new value to store
    function set(uint x) public {
        storedData = x;
        emit ValueChanged(msg.sender, x);
    }
    
    /// @notice Gets the current stored value
    /// @return The current stored value
    function get() public view returns (uint) {
        return storedData;
    }
}