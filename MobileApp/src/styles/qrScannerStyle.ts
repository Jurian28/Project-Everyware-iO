import { StyleSheet } from "react-native";
import colors from "../enums/colors";

export const webStyles = StyleSheet.create({
  container: {
    flex: 1,
    position: 'relative' as any,
    overflow: 'hidden' as any,
  },
  centered: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#0A0A0A',
    padding: 24,
    gap: 12,
  },
  errorIcon: {
    fontSize: 40,
    marginBottom: 8,
  },
  errorText: {
    fontSize: 15,
    textAlign: 'center',
    opacity: 0.8,
    marginBottom: 8,
  },
  bottomOverlay: {
    flex: 1,
    alignItems: 'center',
    paddingTop: 28,
    gap: 12,
  },
  hint: {
    fontSize: 15,
    opacity: 0.85,
    letterSpacing: 0.3,
  },
  button: {
    backgroundColor: colors.DEFAULT_BUTTON_COLOR,
    paddingHorizontal: 28,
    paddingVertical: 12,
    borderRadius: 8,
    marginTop: 4,
  },
  buttonText: {
    color: '#FFF',
    fontWeight: '700',
    fontSize: 15,
  },
  closeButton: {
    paddingHorizontal: 20,
    paddingVertical: 10,
  },
  closeText: {
    color: 'rgba(255,255,255,0.5)',
    fontSize: 14,
  },
});

const CORNER_COLOR = '#00E5BE';
const SCAN_WINDOW_SIZE = 260;
const CORNER_SIZE = 24;
const CORNER_THICKNESS = 4;

export const nativeStyles = StyleSheet.create({
    container: {
        flex: 1,
        minHeight: 400,
        backgroundColor: '#000',
      },
      centered: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: '#0A0A0A',
        padding: 24,
        gap: 12,
      },
      message: {
        color: '#fff',
        fontSize: 16,
        textAlign: 'center',
        marginBottom: 20,
      },
      overlay: {
        ...StyleSheet.absoluteFill,
        flexDirection: 'column',
      },
      topBar: {
        flex: 1,
        backgroundColor: 'rgba(0,0,0,0.55)',
      },
      middleRow: {
        flexDirection: 'row',
        height: SCAN_WINDOW_SIZE,
      },
      sideBar: {
        flex: 1,
        backgroundColor: 'rgba(0,0,0,0.55)',
      },
      scanWindow: {
        width: SCAN_WINDOW_SIZE,
        height: SCAN_WINDOW_SIZE,
      },
      bottomBar: {
        flex: 1,
        backgroundColor: 'rgba(0,0,0,0.55)',
        alignItems: 'center',
        justifyContent: 'center',
        gap: 14,
        paddingBottom: 24,
      },
      hint: {
        color: '#fff',
        fontSize: 15,
        opacity: 0.9,
        letterSpacing: 0.3,
      },
      button: {
        backgroundColor: CORNER_COLOR,
        paddingHorizontal: 28,
        paddingVertical: 12,
        borderRadius: 8,
      },
      buttonText: {
        color: '#000',
        fontWeight: '700',
        fontSize: 15,
      },
      closeButton: {
        paddingHorizontal: 20,
        paddingVertical: 10,
      },
      closeText: {
        color: 'rgba(255,255,255,0.45)',
        fontSize: 14,
      },
      corner: {
        position: 'absolute',
        width: CORNER_SIZE,
        height: CORNER_SIZE,
        borderColor: CORNER_COLOR,
      },
      topLeft:     { top: 0,    left: 0,  borderTopWidth: CORNER_THICKNESS,    borderLeftWidth: CORNER_THICKNESS,  borderTopLeftRadius: 4 },
      topRight:    { top: 0,    right: 0, borderTopWidth: CORNER_THICKNESS,    borderRightWidth: CORNER_THICKNESS, borderTopRightRadius: 4 },
      bottomLeft:  { bottom: 0, left: 0,  borderBottomWidth: CORNER_THICKNESS, borderLeftWidth: CORNER_THICKNESS,  borderBottomLeftRadius: 4 },
      bottomRight: { bottom: 0, right: 0, borderBottomWidth: CORNER_THICKNESS, borderRightWidth: CORNER_THICKNESS, borderBottomRightRadius: 4 },
});