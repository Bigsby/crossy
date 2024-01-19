export enum UnlockType 
{
    Free = "free",
    Prize = "prize",
    Secret = "secret",
    Token = "token",
    Purchase = "purchase"
}

export interface Character
{
    name: string
    category: string
    token: boolean
    secret: boolean
    required: string
    missing: boolean
    type: UnlockType
}

