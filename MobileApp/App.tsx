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
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import {
  ActivityIndicator,
  Pressable,
  StyleSheet,
  Text,
  View,
} from 'react-native';
import EventsStack from './src/navigation/EventsStack';
import HomePage from './src/pages/Home';
import SessionAttendance from './src/pages/attendance/Session';
import LoginPage from './src/pages/auth/Login';
import RegisterPage from './src/pages/auth/Register';
import AuthService from './src/services/AuthService';
import { Image } from 'react-native';

function LogoutPage() {
  const navigator = useNavigation<NavigationProp<ParamListBase>>();

  useEffect(() => {
    const timer = setTimeout(() => {
      AuthService.logout()
        .catch(() => undefined)
        .finally(() => {
          navigator.navigate('Home');
        });
    }, 0);
    return () => clearTimeout(timer);
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

type DrawerToggleProps = {
  readonly navigation: { toggleDrawer: () => void };
  readonly tintColor?: string;
};

function DrawerToggleButton({
  navigation,
  tintColor,
}: Readonly<DrawerToggleProps>) {
  return (
    <Pressable
      accessibilityLabel="Open navigation menu"
      accessibilityRole="button"
      onPress={navigation.toggleDrawer}
      style={styles.drawerToggleButton}
    >
      <Text
        style={[
          styles.drawerToggleIcon,
          tintColor ? { color: tintColor } : null,
        ]}
      >
        {'\u2630'}
      </Text>
    </Pressable>
  );
}

const authenticatedDrawerNavigation = createDrawerNavigator({
  drawerContent: props => <AuthenticatedDrawerContent {...props} />,
  screenOptions: ({ navigation }) => ({
    drawerLabelStyle: {
      color: 'black',
    },
    headerLeft: ({ tintColor }) => (
      <DrawerToggleButton navigation={navigation} tintColor={tintColor} />
    ),
	headerBackground: () => (
		 <View
			 style={{
				 flex: 1,
				 backgroundColor: '#F8F9FA', 
				 borderBottomWidth: StyleSheet.hairlineWidth,
				 borderBottomColor: '#D1D5DB',
				 justifyContent: 'center',
				 alignItems: 'center',
			 }}
		 >
			 <Image
				 source={require('./assets/iOLogo.png')}
				 style={{
					 width: 120,
					 height: 40,
				 }}
				 resizeMode="contain"
			 />
		 </View>
		),
  }),

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

    Events: {
      screen: EventsStack,
      options: {
        title: 'Events',
      },
    },

    AttendanceSession: {
      screen: SessionAttendance,
      options: {
        title: 'Session Attendance Checker',
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
      key={isAuthenticated ? 'auth' : 'unauth'}
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
  drawerToggleButton: {
    paddingHorizontal: 12,
    paddingVertical: 8,
  },
  drawerToggleIcon: {
    color: '#111827',
    fontSize: 20,
    lineHeight: 20,
  },
});
