export interface CharacterResponse<T> {
    isSuccess: boolean,
    errorMessage: string,
    data: T
}