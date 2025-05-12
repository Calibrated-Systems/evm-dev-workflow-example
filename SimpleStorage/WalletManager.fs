module SimpleStorage.WalletManager

open System
open System.IO
open System.Text
open System.Numerics
open Nethereum.Web3
open Nethereum.Web3.Accounts
open Nethereum.KeyStore
open Nethereum.KeyStore.Model
open Nethereum.HdWallet
open NBitcoin
open Newtonsoft.Json

/// <summary>
/// Represents a wallet configuration for storage
/// </summary>
type WalletConfig = {
    Mnemonic: string
    Password: string
}

/// <summary>
/// Generates a random password that is reasonably easy to type
/// Considers Dvorak keyboard layout with Australian locality
/// </summary>
let generatePassword () =
    // Characters that are easy to type on Dvorak keyboard layout
    let easyChars = "aoeuidhtns-_"
    let numbers = "0123456789"
    let specialChars = ".,;:"
    
    let random = Random()
    let sb = StringBuilder()
    
    // Add 8 easy characters
    for _ in 1..8 do
        sb.Append(easyChars.[random.Next(easyChars.Length)]) |> ignore
    
    // Add 2 numbers
    for _ in 1..2 do
        sb.Append(numbers.[random.Next(numbers.Length)]) |> ignore
    
    // Add 2 special characters
    for _ in 1..2 do
        sb.Append(specialChars.[random.Next(specialChars.Length)]) |> ignore
    
    sb.ToString()

/// <summary>
/// Creates a new HD wallet and returns the account and wallet configuration
/// </summary>
let createWallet () =
    // Create a new wallet with a random mnemonic (12 words)
    let words = Mnemonic(Wordlist.English, WordCount.Twelve).Words
    let mnemonic = String.Join(" ", words)
    
    // Create the wallet with the mnemonic
    let wallet = new Wallet(mnemonic, null)
    
    // Get the account at index 0
    let account = wallet.GetAccount(0)
    
    // Generate a password for encryption
    let password = generatePassword()
    
    // Create wallet config
    let walletConfig = {
        Mnemonic = mnemonic
        Password = password
    }
    
    (account, walletConfig)

/// <summary>
/// Saves a wallet configuration to a file and returns the file path
/// </summary>
let saveWallet (account: Account) (walletConfig: WalletConfig) (keystorePath: string) =
    // Ensure directory exists
    Directory.CreateDirectory(keystorePath) |> ignore
    
    // Create keystore service
    let keyStoreService = new KeyStoreService()
    
    // Generate keystore JSON for the private key
    let privateKeyBytes = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(account.PrivateKey)
    let keyStoreJson = keyStoreService.EncryptAndGenerateDefaultKeyStoreAsJson(
        walletConfig.Password,
        privateKeyBytes,
        account.Address)
    
    // Create file name based on address
    let keystoreFileName = $"wallet-{account.Address}.json"
    let keystoreFilePath = Path.Combine(keystorePath, keystoreFileName)
    
    // Write keystore to file
    File.WriteAllText(keystoreFilePath, keyStoreJson)
    
    // Create wallet config file name
    let configFileName = "wallet-config.json"
    let configFilePath = Path.Combine(keystorePath, configFileName)
    
    // Serialize wallet config to JSON
    let configJson = JsonConvert.SerializeObject(walletConfig, Formatting.Indented)
    
    // Write wallet config to file
    File.WriteAllText(configFilePath, configJson)
    
    // Save wallet config path to environment variable
    Environment.SetEnvironmentVariable("WALLET_CONFIG_PATH", configFilePath, EnvironmentVariableTarget.User)
    
    // Return file path
    configFilePath

/// <summary>
/// Loads a wallet from a wallet configuration file
/// </summary>
let loadWalletFromConfig (configFilePath: string) =
    // Read wallet config JSON
    let configJson = File.ReadAllText(configFilePath)
    
    // Deserialize wallet config
    let walletConfig = JsonConvert.DeserializeObject<WalletConfig>(configJson)
    
    // Create wallet from mnemonic
    let wallet = new Wallet(walletConfig.Mnemonic, null)
    
    // Get account at index 0
    wallet.GetAccount(0)

/// <summary>
/// Loads a wallet from a keystore file (legacy method)
/// </summary>
let loadWalletFromKeystore (filePath: string) (password: string) =
    // Read keystore JSON
    let keyStoreJson = File.ReadAllText(filePath)
    
    // Create account from keystore
    Account.LoadFromKeyStore(keyStoreJson, password)

/// <summary>
/// Tries to load a wallet from environment variables
/// Returns Some account if successful, None otherwise
/// </summary>
let tryLoadWalletFromEnv () =
    // First try to load from wallet config
    let configPath = Environment.GetEnvironmentVariable("WALLET_CONFIG_PATH", EnvironmentVariableTarget.User)
    
    if not (String.IsNullOrEmpty(configPath)) && File.Exists(configPath) then
        try
            Some(loadWalletFromConfig configPath)
        with
        | _ -> None
    else
        // Fall back to legacy keystore method
        let walletPath = Environment.GetEnvironmentVariable("WALLET_PATH", EnvironmentVariableTarget.User)
        let password = Environment.GetEnvironmentVariable("WALLET_PASSWORD", EnvironmentVariableTarget.User)
        
        if String.IsNullOrEmpty(walletPath) || String.IsNullOrEmpty(password) then
            None
        else
            try
                Some(loadWalletFromKeystore walletPath password)
            with
            | _ -> None

/// <summary>
/// Gets or creates a wallet
/// If a wallet exists in environment variables, it will be loaded
/// Otherwise, a new wallet will be created and saved
/// </summary>
let getOrCreateWallet (keystorePath: string) =
    match tryLoadWalletFromEnv() with
    | Some account -> 
        printfn "Loaded existing wallet from environment variables"
        account
    | None ->
        printfn "Creating new HD wallet..."
        let (account, walletConfig) = createWallet()
        let filePath = saveWallet account walletConfig keystorePath
        printfn "HD Wallet created and saved to %s" filePath
        printfn "Mnemonic phrase: %s" walletConfig.Mnemonic
        printfn "Address: %s" account.Address
        account