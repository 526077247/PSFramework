export class CookieUtil {
  private pluses = /\+/g;
  private defaults = {};
  private raw: boolean;
  private json: boolean;

  encode(s) {
    return this.raw ? s : encodeURIComponent(s);
  }

  decode(s) {
    return this.raw ? s : decodeURIComponent(s);
  }

  stringifyCookieValue(value) {
    return this.encode(this.json ? JSON.stringify(value) : String(value));
  }

  parseCookieValue(s) {
    if (s.indexOf('"') === 0) {
      // This is a quoted cookie as according to RFC2068, unescape...
      s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
    }

    try {
      // Replace server-side written pluses with spaces.
      // If we can't decode the cookie, ignore it, it's unusable.
      // If we can't parse the cookie, ignore it, it's unusable.
      s = decodeURIComponent(s.replace(this.pluses, ' '));
      return this.json ? JSON.parse(s) : s;
    } catch (e) {
    }
  }

  read(s, converter?: any) {
    const value = this.raw ? s : this.parseCookieValue(s);
    return typeof converter === 'function' ? converter(value) : value;
  }

  cookie(key, value?: any, options?: any): any {

    // Write

    if (arguments.length > 1 && typeof value !== 'function') {
      options = Object.assign({}, this.defaults, options);

      if (typeof options.expires === 'number') {
        const seconds = options.expires, t = options.expires = new Date();
        t.setMilliseconds(t.getMilliseconds() + seconds * 1000);
      }
      return (document.cookie = [
        this.encode(key), '=', this.stringifyCookieValue(value),
        options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
        options.path ? '; path=' + options.path : '',
        options.domain ? '; domain=' + options.domain : '',
        options.secure ? '; secure' : ''
      ].join(''));
    }

    // Read

    let result = key ? undefined : {},
      i = 0;
    // To prevent the for loop in the first place assign an empty array
    // in case there are no cookies at all. Also prevents odd result when
    // calling $.cookie().
    const cookies = document.cookie ? document.cookie.split('; ') : [],
      l = cookies.length;

    for (; i < l; i++) {
      const parts = cookies[i].split('='),
        name = this.decode(parts.shift());
      let cookie = parts.join('=');

      if (key === name) {
        // If second argument (value) is a function it's a converter...
        result = this.read(cookie, value);
        break;
      }

      // Prevent storing a cookie that we couldn't decode.
      if (!key && (cookie = this.read(cookie)) !== undefined) {
        result[name] = cookie;
      }
    }

    return result;
  }

  removeCookie(key, options = {}): boolean {
    // Must not alter options, thus extending a fresh object...
    this.cookie(key, '', Object.assign({}, options, {expires: -1}));
    return !this.cookie(key);
  }
}
