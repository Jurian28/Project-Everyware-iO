// Web stub for react-native-keychain
const Keychain = {
    getGenericPassword: async () => null,
    setGenericPassword: async () => true,
    resetGenericPassword: async () => true,
    getSupportedBiometryType: async () => null,
};

export default Keychain;
export const getGenericPassword = Keychain.getGenericPassword;
export const setGenericPassword = Keychain.setGenericPassword;
export const resetGenericPassword = Keychain.resetGenericPassword;
export const getSupportedBiometryType = Keychain.getSupportedBiometryType;