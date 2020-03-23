<template>
  <div class="empty">
    <slot />
  </div>
</template>

<style lang="scss" scoped>
@import '@/theme/colors.scss';

.empty {
  padding: 16px;
  border: 1px dashed #8c8c8c;
  border-radius: 4px;
  text-align: center;
  color: #8c8c8c;
}
</style>