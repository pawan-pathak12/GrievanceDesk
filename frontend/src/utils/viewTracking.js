const STORAGE_PREFIX = 'grievance-desk:view-tracking';
const VIEW_STATE_EVENT = 'grievance-desk:view-state-changed';

function getStorageKey(userKey) {
  return `${STORAGE_PREFIX}:${userKey || 'member'}`;
}

function getVersion(item, type) {
  if (type === 'notices') {
    return String(item.publishedAt || item.updatedAt || item.createdAt || item.id);
  }

  return String(item.updatedAt || item.createdAt || item.id);
}

function readState(userKey) {
  try {
    return JSON.parse(localStorage.getItem(getStorageKey(userKey))) || {};
  } catch {
    return {};
  }
}

function writeState(userKey, state) {
  localStorage.setItem(getStorageKey(userKey), JSON.stringify(state));
  window.dispatchEvent(new Event(VIEW_STATE_EVENT));
}

export function getUnreadCount(userKey, type, items) {
  const viewedItems = readState(userKey)[type] || {};
  return items.filter((item) => viewedItems[item.id] !== getVersion(item, type)).length;
}

export function markItemsViewed(userKey, type, items) {
  const state = readState(userKey);
  const viewedItems = state[type] || {};
  let changed = false;

  items.forEach((item) => {
    const version = getVersion(item, type);
    if (viewedItems[item.id] !== version) {
      viewedItems[item.id] = version;
      changed = true;
    }
  });

  if (changed) {
    writeState(userKey, { ...state, [type]: viewedItems });
  }
}

export const viewStateEvent = VIEW_STATE_EVENT;
