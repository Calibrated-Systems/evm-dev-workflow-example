# EvmDevWorkflowExample - F# Nethereum Project

This project is an F# implementation of the Nethereum workflow example, demonstrating how to create, deploy, and interact with Ethereum smart contracts using F# and Nethereum.

## Project Overview

- Uses the latest Solidity version (^0.8.19) for the smart contract
- Connects to the AVAX Fuji Testnet
- Creates and securely saves a Nethereum wallet using the standard keystore format
- Implements a simple storage contract with get and set functions
- Uses F# generated code to interact with the smart contract

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [Visual Studio Code](https://code.visualstudio.com/) with the [Solidity extension](https://marketplace.visualstudio.com/items?itemName=JuanBlanco.solidity)
- [Visual Studio](https://visualstudio.microsoft.com/) with F# support

## Project Structure

```
EvmDevWorkflowExample/
├── EvmDevWorkflowExample.sln
├── SimpleStorage/
│   ├── SimpleStorage.fsproj
│   ├── Program.fs
│   ├── WalletManager.fs
│   ├── Contracts/
│   │   └── SimpleStorage/
│   │       ├── SimpleStorage.sol
│   │       ├── SimpleStorageService.gen.fs
│   │       └── ContractDefinition/
│   │           └── SimpleStorageDefinition.gen.fs
│   ├── Wallet/
│   │   └── keystore/ (for encrypted wallet files)
```

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio Code or Visual Studio
3. Build the solution: `dotnet build`
4. Fund your wallet with test AVAX tokens (see AVAX Fuji Testnet section below)

## Workflow

### 1. Compile the Smart Contract

Use the Solidity extension in VSCode to compile the SimpleStorage.sol contract:
- Open the SimpleStorage/Contracts/SimpleStorage/SimpleStorage.sol file
- Press Shift+Ctrl+P and select "Solidity: Compile Contract"

This will generate the ABI and bytecode files needed for the next step.

### 2. Generate F# Service Code

In Visual Studio Code:
- Open the solution
- Press Shift+Ctrl+P and select "Solidity: Code generate FSharp Definitions in Abi files in current folder"

This will generate the F# service code needed to interact with the contract.

### 3. Run the Application

Run the application:
```
dotnet run --project SimpleStorage
```

The application will:
1. Create a new wallet or load an existing one
2. Connect to the AVAX Fuji Testnet
3. Deploy the SimpleStorage contract
4. Call the set function to store a value
5. Call the get function to retrieve the value

## Wallet Management

The application uses Nethereum's keystore functionality to securely store the wallet:
- The wallet is encrypted with a password and stored in the keystore directory
- The password is stored as an environment variable for development purposes
- The wallet path is also stored as an environment variable

## AVAX Fuji Testnet

The application connects to the AVAX Fuji Testnet:
- RPC URL: https://api.avax-test.network/ext/bc/C/rpc
- Chain ID: 43113

To get test AVAX tokens, use the [AVAX Faucet](https://faucet.avax.network/).

**Important**: You need to fund your wallet with test AVAX tokens before you can deploy contracts or send transactions. The application will show an "insufficient funds" error if your wallet doesn't have enough tokens.

## Notes

- This is a development example and should not be used in production without proper security measures
- The wallet password is stored as an environment variable for convenience, but in a production environment, it should be handled more securely