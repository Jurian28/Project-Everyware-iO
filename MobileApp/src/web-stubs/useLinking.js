export function useLinking() {
    return { getInitialState: () => Promise.resolve(undefined) };
}
export default useLinking;