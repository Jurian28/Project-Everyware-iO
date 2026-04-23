import {
  createDrawerNavigator,
  DrawerContentScrollView,
  DrawerItem,
  DrawerItemList,
  type DrawerContentComponentProps,
} from '@react-navigation/drawer';
import {
  createStaticNavigation,
  useNavigation,
  type NavigationProp,
  type ParamListBase,
} from '@react-navigation/native';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { ActivityIndicator, StyleSheet, Text, View } from 'react-native';
import HomePage from './src/pages/Home';
import LoginPage from './src/pages/auth/login';
import RegisterPage from './src/pages/auth/register';
import AuthService from './src/services/AuthService';

function LogoutPage() {
  const navigator = useNavigation<NavigationProp<ParamListBase>>();

  useEffect(() => {
    AuthService.logout()
      .catch(() => undefined)
      .finally(() => {
        navigator.navigate('Home');
      });
  }, [navigator]);

  return (
    <View style={styles.loaderContainer}>
      <ActivityIndicator size="large" color="black" />
      <Text style={styles.loaderText}>Signing out...</Text>
    </View>
  );
}

function AuthenticatedDrawerContent(
  props: Readonly<DrawerContentComponentProps>,
) {
  return (
    <DrawerContentScrollView {...props}>
      <DrawerItemList {...props} />
      <View style={styles.drawerSeparator} />
      <DrawerItem
        label="Logout"
        labelStyle={styles.logoutLabel}
        onPress={() => props.navigation.navigate('Logout')}
      />
    </DrawerContentScrollView>
  );
}

const authenticatedDrawerNavigation = createDrawerNavigator({
  drawerContent: props => <AuthenticatedDrawerContent {...props} />,
  screenOptions: {
    drawerLabelStyle: {
      color: 'black',
    },
  },

  screens: {
    Home: {
      screen: HomePage,
      options: {
        title: 'Home',
      },
    },

    Logout: {
      screen: LogoutPage,
      options: {
        title: 'Logout',
        headerShown: false,
        drawerItemStyle: {
          display: 'none',
        },
      },
    },
  },
});

const unauthenticatedDrawerNavigation = createDrawerNavigator({
  screenOptions: {
    headerShown: false,
    swipeEnabled: false,
    drawerType: 'back',
  },
  initialRouteName: 'Login',
  screens: {
    Login: {
      screen: LoginPage,
      options: {
        title: 'Login page',
        drawerItemStyle: {
          display: 'none',
        },
      },
    },
    Register: {
      screen: RegisterPage,
      options: {
        title: 'Register page',
        drawerItemStyle: {
          display: 'none',
        },
      },
    },
    Home: {
      screen: HomePage,
      options: {
        title: 'Home page',
        drawerItemStyle: {
          display: 'none',
        },
      },
    },
  },
});

const AuthenticatedNavigation = createStaticNavigation(
  authenticatedDrawerNavigation,
);
const UnauthenticatedNavigation = createStaticNavigation(
  unauthenticatedDrawerNavigation,
);

export default function App() {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);

  const refreshAuthentication = useCallback(async () => {
    const authenticated = await AuthService.isAuthenticated();
    setIsAuthenticated(authenticated);
  }, []);

  useEffect(() => {
    refreshAuthentication().catch(() => undefined);
  }, [refreshAuthentication]);

  const handleNavigationStateChange = useCallback(() => {
    refreshAuthentication().catch(() => undefined);
  }, [refreshAuthentication]);

  const NavigationComponent = useMemo(
    () =>
      isAuthenticated ? AuthenticatedNavigation : UnauthenticatedNavigation,
    [isAuthenticated],
  );

  if (isAuthenticated === null) {
    return (
      <View style={styles.loaderContainer}>
        <ActivityIndicator size="large" color="black" />
      </View>
    );
  }

  return (
    <NavigationComponent
      onReady={handleNavigationStateChange}
      onStateChange={handleNavigationStateChange}
    />
  );
}

const styles = StyleSheet.create({
  loaderContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  loaderText: {
    marginTop: 12,
    color: '#2a3344',
  },
  drawerSeparator: {
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: '#bcc5d1',
    marginHorizontal: 16,
    marginTop: 10,
    marginBottom: 4,
  },
  logoutLabel: {
    color: '#111827',
  },
});
