import { createDrawerNavigator } from '@react-navigation/drawer';
import { createStaticNavigation } from '@react-navigation/native';
import HomePage from './src/pages/Home';
import LoginPage from './src/pages/auth/login';
import RegisterPage from './src/pages/auth/register';

const drawerNavigation = createDrawerNavigator({
  screenOptions: {
    drawerLabelStyle: {
      color: 'black',
    },
  },

  screens: {
    Home: {
      screen: HomePage,
      options: {
        title: 'Home page',
      },
    },

    Register: {
      screen: RegisterPage,
      options: {
        title: 'Register page',
      },
    },

    Login: {
      screen: LoginPage,
      options: {
        title: 'Login page',
      },
    },
  },
});
const Navigation = createStaticNavigation(drawerNavigation);

export default function App() {
  return <Navigation />;
}
