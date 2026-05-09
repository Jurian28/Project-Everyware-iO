const Keychain = {
    getGenericPassword: async (options) => {
        const service = options?.service || 'default';
        const key = `keychain_${service}`;
        const stored = localStorage.getItem(key);
        if (!stored) return null;
        const { username, password } = JSON.parse(stored);
        return { username, password };
    },
    setGenericPassword: async (username, password, options) => {
        const service = options?.service || 'default';
        const key = `keychain_${service}`;
        localStorage.setItem(key, JSON.stringify({ username, password }));
        return true;
    },
    resetGenericPassword: async (options) => {
        const service = options?.service || 'default';
        const key = `keychain_${service}`;
        localStorage.removeItem(key);
        return true;
    },
    getSupportedBiometryType: async () => null,
};

export default Keychain;
export const getGenericPassword = Keychain.getGenericPassword;
export const setGenericPassword = Keychain.setGenericPassword;
export const resetGenericPassword = Keychain.resetGenericPassword;
export const getSupportedBiometryType = Keychain.getSupportedBiometryType;
